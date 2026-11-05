using System.Net;
using Kahrad.Pricing.Sms.Application.Abstractions.Persistence;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.DTOs;
using Kahrad.Pricing.Sms.Application.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Kahrad.Pricing.Sms.Domain.Entities;
using Kahrad.Pricing.Sms.Domain.Enums;
using Kahrad.Pricing.Sms.Infrastructure.Options;
using Kahrad.Pricing.Sms.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace Kahrad.Pricing.Sms.Tests;

public sealed class SmsDeliveryTests
{
    private static readonly DateTime Now = new(2026, 11, 5, 0, 0, 0, DateTimeKind.Utc);
    private static SmsMessage Message() => SmsMessage.CreateBulk(Guid.NewGuid(), "09121234567", "test", Now);

    [Fact]
    public void Retry_is_delayed_without_changing_original_schedule()
    {
        var message = Message();
        message.MarkAsSending(Now);
        message.MarkAsFailed("busy", true, Now, 3, TimeSpan.FromMinutes(2));
        Assert.Equal(SmsStatus.Pending, message.Status);
        Assert.Equal(Now, message.SendDateUtc);
        Assert.False(message.IsReadyToSend(Now.AddMinutes(1)));
        Assert.True(message.IsReadyToSend(Now.AddMinutes(2)));
    }

    [Theory]
    [InlineData(false, 3)]
    [InlineData(true, 1)]
    public void Unsafe_or_exhausted_failures_are_terminal(bool retryable, int attempts)
    {
        var message = Message();
        message.MarkAsSending(Now);
        message.MarkAsFailed("rejected", retryable, Now, attempts, TimeSpan.FromMinutes(2));
        Assert.Equal(SmsStatus.Failed, message.Status);
    }

    [Fact]
    public void Retry_cannot_extend_message_expiration()
    {
        var message = Message();
        message.MarkAsSending(Now);
        message.MarkAsFailed("busy", true, Now.AddHours(24), 3, TimeSpan.FromMinutes(2));
        Assert.Equal(SmsStatus.Failed, message.Status);
    }

    [Fact]
    public void Backoff_is_exponential_and_capped()
    {
        var retry = new SmsModuleConfiguration(1, initialRetryDelaySeconds: 10, maxRetryDelaySeconds: 30);
        Assert.Equal(10, retry.GetRetryDelay(1).TotalSeconds);
        Assert.Equal(20, retry.GetRetryDelay(2).TotalSeconds);
        Assert.Equal(30, retry.GetRetryDelay(3).TotalSeconds);
    }

    [Theory]
    [InlineData(409, true)]
    [InlineData(403, false)]
    public async Task Kavenegar_classifies_confirmed_rejections(int status, bool retryable)
    {
        using var transport = new KavenegarTransport(new HttpClient(new Handler($"{{\"return\":{{\"status\":{status}}},\"entries\":null}}")));
        var provider = new KavenegarSmsProvider(transport, Options.Create(new KavenegarOptions { ApiKey = "test" }));
        var result = await provider.SendBulkAsync(["09121234567"], "test");
        Assert.False(result.IsSuccess);
        Assert.Equal(retryable, result.CanRetry);
    }

    [Fact]
    public async Task Kavenegar_reads_receipt_and_cost()
    {
        using var transport = new KavenegarTransport(new HttpClient(new Handler("{\"return\":{\"status\":200},\"entries\":[{\"messageid\":123,\"cost\":100}]}")));
        var provider = new KavenegarSmsProvider(transport, Options.Create(new KavenegarOptions { ApiKey = "test" }));
        var result = await provider.SendBulkAsync(["09121234567"], "test");
        Assert.True(result.IsSuccess);
        Assert.Equal("123", result.ProviderMessageId);
        Assert.Equal(100m, result.Cost);
    }

    [Fact]
    public async Task Kavenegar_requires_template_mapping_without_sending()
    {
        using var transport = new KavenegarTransport(new HttpClient(new Handler("invalid")));
        var provider = new KavenegarSmsProvider(transport, Options.Create(new KavenegarOptions { ApiKey = "test" }));
        Assert.False((await provider.SendTemplateAsync("09121234567", 1, new Dictionary<string, string>())).IsSuccess);
    }

    [Theory]
    [InlineData(true, true, SmsStatus.Pending)]
    [InlineData(false, true, SmsStatus.Failed)]
    [InlineData(true, false, SmsStatus.Failed)]
    public async Task Outbox_persists_only_configured_safe_retries(bool enabled, bool safe, SmsStatus expected)
    {
        var message = SmsMessage.CreateBulk(Guid.NewGuid(), "09121234567", "test", DateTime.UtcNow);
        var configuration = Persisted(new SmsProviderConfiguration("Primary", "Kavenegar", "Sms:Providers:Kavenegar:ApiKey"));
        Persisted(message);
        message.AssignProvider(configuration);
        var repository = new Repository(message);
        var provider = new Provider(SmsProviderResult.Failure("busy", safe));
        var resolver = new Resolver(provider);
        var processor = new SmsOutboxProcessor(repository, resolver,
            new Configurations(configuration, new SmsModuleConfiguration(1, retryEnabled: enabled)),
            NullLogger<SmsOutboxProcessor>.Instance);
        var result = await processor.ProcessPendingOutboxAsync();
        Assert.Equal(expected, message.Status);
        Assert.Equal("Kavenegar", resolver.Resolved);
        Assert.Equal(2, repository.Updates);
        Assert.Equal(1, result.FailedCount);
        Assert.Equal(1, message.RetryCount);
        var log = Assert.Single(repository.Logs);
        Assert.Equal(SmsLogStatus.Failed, log.Status);
        Assert.Equal("ProviderRejected", log.ErrorCode);
        Assert.Equal(1, log.AttemptNumber);
        Assert.NotNull(log.CompletedAtUtc);
    }

    [Fact]
    public async Task Missing_smsir_credentials_do_not_report_success()
    {
        var provider = new IpeSmsProvider(Options.Create(new SmsIrOptions()), NullLogger<IpeSmsProvider>.Instance);
        Assert.False((await provider.SendBulkAsync(["09121234567"], "test")).IsSuccess);
    }

    [Fact]
    public void Unknown_log_outcome_cannot_be_retried_or_completed_twice()
    {
        var log = new SmsLog(1, 1, 1, Now);
        log.Complete(false, true, null, null, true, Now.AddSeconds(1));
        Assert.Equal(SmsLogStatus.Unknown, log.Status);
        Assert.False(log.CanRetry);
        Assert.Equal("DeliveryOutcomeUnknown", log.ErrorCode);
        Assert.Throws<InvalidOperationException>(() => log.Complete(true, false, "123", 10, false, Now.AddSeconds(2)));
    }

    [Theory]
    [InlineData(0, 3, 10, 30)]
    [InlineData(201, 3, 10, 30)]
    [InlineData(100, 0, 10, 30)]
    [InlineData(100, 3, 30, 10)]
    public void Module_settings_reject_invalid_values(int batch, int attempts, int initial, int maximum)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SmsModuleConfiguration(1,
            batchSize: batch, maxAttempts: attempts, initialRetryDelaySeconds: initial, maxRetryDelaySeconds: maximum));
    }

    [Fact]
    public void Provider_templates_are_validated_and_returned_as_a_copy()
    {
        var provider = new SmsProviderConfiguration("Primary", "Kavenegar", "Sms:Providers:Kavenegar:ApiKey");
        provider.SetTemplateMappings(new Dictionary<int, string> { [1] = "verify" });
        provider.GetTemplateMappings()[1] = "changed";
        Assert.Equal("verify", provider.GetTemplateMappings()[1]);
        Assert.Throws<ArgumentException>(() => provider.SetTemplateMappings(new Dictionary<int, string> { [0] = "" }));
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task Disabling_module_or_provider_pauses_delivery(bool moduleEnabled, bool providerEnabled)
    {
        var configuration = Persisted(new SmsProviderConfiguration("Primary", "Kavenegar", "Sms:Providers:Kavenegar:ApiKey"));
        var message = Persisted(SmsMessage.CreateBulk(Guid.NewGuid(), "09121234567", "test", DateTime.UtcNow));
        message.AssignProvider(configuration);
        configuration.Configure(configuration.ApiKeyConfigurationKey, null, 30, providerEnabled);
        var repository = new Repository(message);
        var processor = new SmsOutboxProcessor(repository, new Resolver(new Provider(SmsProviderResult.Success("123"))),
            new Configurations(configuration, new SmsModuleConfiguration(1, isEnabled: moduleEnabled)),
            NullLogger<SmsOutboxProcessor>.Instance);
        await processor.ProcessPendingOutboxAsync();
        Assert.Equal(SmsStatus.Pending, message.Status);
        Assert.Empty(repository.Logs);
        Assert.Equal(0, repository.Updates);
    }

    private sealed class Resolver(ISmsProvider provider) : ISmsProviderResolver
    {
        public string? Resolved { get; private set; }
        public ISmsProvider Resolve(SmsProviderConfiguration configuration) { Resolved = configuration.Provider; return provider; }
    }

    private sealed class Provider(SmsProviderResult result) : ISmsProvider
    {
        public Task<SmsProviderResult> SendBulkAsync(IEnumerable<string> phones, string body, string? lineNumber = null, CancellationToken cancellationToken = default) => Task.FromResult(result);
        public Task<SmsProviderResult> SendPeerToPeerAsync(IEnumerable<PeerToPeerSmsItem> items, string? lineNumber = null, CancellationToken cancellationToken = default) => Task.FromResult(result);
        public Task<SmsProviderResult> SendTemplateAsync(string phone, int template, IDictionary<string, string> parameters, string? lineNumber = null, CancellationToken cancellationToken = default) => Task.FromResult(result);
    }

    private sealed class Repository(SmsMessage message) : ISmsMessageRepository
    {
        public int Updates { get; private set; }
        public List<SmsLog> Logs { get; } = [];
        public Task<IReadOnlyCollection<SmsMessage>> GetPendingMessagesAsync(int batchSize, DateTime now, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<SmsMessage>>([message]);
        public Task<IReadOnlyCollection<SmsMessage>> GetExpiredPendingMessagesAsync(DateTime now, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<SmsMessage>>([]);
        public Task UpdateAsync(SmsMessage value, CancellationToken cancellationToken = default) { Updates++; return Task.CompletedTask; }
        public Task UpdateRangeAsync(IEnumerable<SmsMessage> values, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<SmsMessage?> GetByIdAsync(long id, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<SmsMessage?> GetByPublicIdAsync(Guid id, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyCollection<SmsMessage>> GetByBatchIdAsync(Guid id, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<long> AddAsync(SmsMessage value, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task AddRangeAsync(IEnumerable<SmsMessage> values, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task StartAttemptAsync(SmsMessage value, SmsLog log, CancellationToken cancellationToken = default) { Logs.Add(log); return UpdateAsync(value, cancellationToken); }
        public Task CompleteAttemptAsync(SmsMessage value, SmsLog log, CancellationToken cancellationToken = default) => UpdateAsync(value, cancellationToken);
        public Task<IReadOnlyCollection<SmsLog>> GetLogsAsync(long id, int page, int pageSize, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public IQueryable<SmsMessage> GetQueryable() => throw new NotSupportedException();
    }

    private static T Persisted<T>(T entity) where T : Kahrad.Pricing.BuildingBlocks.Domain.Common.BaseEntity
    {
        typeof(Kahrad.Pricing.BuildingBlocks.Domain.Common.BaseEntity).GetProperty("Id")!.SetValue(entity, 1L);
        return entity;
    }

    private sealed class Configurations(SmsProviderConfiguration provider, SmsModuleConfiguration module) : ISmsConfigurationRepository
    {
        public Task<SmsModuleConfiguration> GetModuleAsync(CancellationToken cancellationToken = default) => Task.FromResult(module);
        public Task<SmsProviderConfiguration> GetProviderAsync(long id, CancellationToken cancellationToken = default) => Task.FromResult(provider);
        public Task<IReadOnlyCollection<SmsProviderConfiguration>> GetProvidersAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyCollection<SmsProviderConfiguration>>([provider]);
        public Task<long> AddProviderAsync(SmsProviderConfiguration value, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task UpdateModuleAsync(SmsModuleConfiguration value, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task UpdateProviderAsync(SmsProviderConfiguration value, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class Handler(string json) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });
    }
}
