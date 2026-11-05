using Kahrad.Pricing.Sms.Domain.Entities;
using Kahrad.Pricing.Sms.Domain.Enums;
using Kahrad.Pricing.Sms.Infrastructure.Persistence;
using Kahrad.Pricing.Sms.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Kahrad.Pricing.Sms.IntegrationTests;

public sealed class SmsPersistenceTests
{
    [SqlServerFact]
    public async Task Message_and_attempt_log_are_persisted_with_provider_configuration()
    {
        var connection = Environment.GetEnvironmentVariable("SMS_TEST_CONNECTION")!;
        await using var context = new SmsDbContext(new DbContextOptionsBuilder<SmsDbContext>().UseSqlServer(connection).Options);
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var configurations = new SmsConfigurationRepository(context);
            var module = await configurations.GetModuleAsync();
            var provider = await configurations.GetProviderAsync(module.ActiveProviderConfigurationId);
            var message = SmsMessage.CreateBulk(Guid.NewGuid(), "09120000000", "integration test", DateTime.UtcNow);
            message.AssignProvider(provider);
            var messages = new SmsMessageRepository(context);
            await messages.AddAsync(message);
            var started = DateTime.UtcNow;
            var log = new SmsLog(message.Id, provider.Id, 1, started);
            message.MarkAsSending(started);
            await messages.StartAttemptAsync(message, log);
            context.ChangeTracker.Clear();
            Assert.Equal(SmsStatus.Sending, (await context.Messages.SingleAsync(x => x.Id == message.Id)).Status);
            Assert.Equal(SmsLogStatus.Started, (await context.Logs.SingleAsync(x => x.Id == log.Id)).Status);
            context.ChangeTracker.Clear();
            message.MarkAsSent("test-receipt", 100, DateTime.UtcNow);
            log.Complete(true, false, "test-receipt", 100, false, DateTime.UtcNow);
            await messages.CompleteAttemptAsync(message, log);
            context.ChangeTracker.Clear();
            var savedMessage = await context.Messages.SingleAsync(x => x.Id == message.Id);
            var savedLog = Assert.Single(await messages.GetLogsAsync(message.Id, 1, 20));
            Assert.Equal(SmsStatus.Sent, savedMessage.Status);
            Assert.Equal(provider.Id, savedMessage.ProviderConfigurationId);
            Assert.Equal(SmsLogStatus.Succeeded, savedLog.Status);
            Assert.Equal("test-receipt", savedLog.ProviderMessageId);
            Assert.Equal(100m, savedLog.Cost);
        }
        finally { await transaction.RollbackAsync(); }
    }
}

public sealed class SqlServerFactAttribute : FactAttribute
{
    public SqlServerFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SMS_TEST_CONNECTION")))
            Skip = "Set SMS_TEST_CONNECTION to run against an explicitly selected SQL Server database.";
    }
}
