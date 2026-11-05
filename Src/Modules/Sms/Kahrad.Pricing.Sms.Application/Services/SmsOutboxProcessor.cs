using System.Text.Json;
using Kahrad.Pricing.Sms.Application.Abstractions.Persistence;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.DTOs;
using Kahrad.Pricing.Sms.Domain.Entities;
using Kahrad.Pricing.Sms.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Kahrad.Pricing.Sms.Application.Services;

public sealed class SmsOutboxProcessor(
    ISmsMessageRepository repository,
    ISmsProviderResolver providers,
    ISmsConfigurationRepository configurations,
    ILogger<SmsOutboxProcessor> logger) : ISmsOutboxProcessor
{
    public async Task<SmsOutboxProcessResult> ProcessPendingOutboxAsync(CancellationToken cancellationToken = default)
    {
        var result = new SmsOutboxProcessResult();
        var module = await configurations.GetModuleAsync(cancellationToken);
        if (!module.IsEnabled) return result;
        var now = DateTime.UtcNow;
        var expired = await repository.GetExpiredPendingMessagesAsync(now, cancellationToken);
        foreach (var message in expired) message.MarkAsExpired(now);
        if (expired.Count > 0) await repository.UpdateRangeAsync(expired, cancellationToken);
        result.ExpiredCount = expired.Count;

        var pending = await repository.GetPendingMessagesAsync(module.BatchSize, now, cancellationToken);
        var providerConfigurations = (await configurations.GetProvidersAsync(
            pending.Select(x => x.ProviderConfigurationId), cancellationToken)).ToDictionary(x => x.Id);
        foreach (var message in pending)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!message.IsReadyToSend(DateTime.UtcNow)) continue;
            // RowVersion makes this a compare-and-swap claim. A competing processor cannot send this row.
            if (!providerConfigurations.TryGetValue(message.ProviderConfigurationId, out var providerConfiguration))
                continue;
            // Disabling a provider pauses its queue, rather than failing or rerouting messages.
            if (!providerConfiguration.IsEnabled) continue;
            var startedAt = DateTime.UtcNow;
            var log = new SmsLog(message.Id, providerConfiguration.Id, message.RetryCount + 1, startedAt);
            message.MarkAsSending(startedAt);
            await repository.StartAttemptAsync(message, log, cancellationToken);
            SmsProviderResult response;
            try
            {
                response = await SendAsync(message, providerConfiguration, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
            catch (Exception)
            {
                // Provider exceptions may include secrets or message bodies; do not log their payload.
                logger.LogWarning("SMS provider failed for message {MessageId}; delivery outcome requires investigation.", message.Id);
                response = SmsProviderResult.Failure("Delivery outcome unknown; investigate before resending.", outcomeUnknown: true);
            }
            now = DateTime.UtcNow;
            if (response.IsSuccess)
            {
                message.MarkAsSent(response.ProviderMessageId, response.Cost, now);
                result.SentCount++;
            }
            else
            {
                message.MarkAsFailed(response.ErrorMessage ?? "SMS provider rejected request.",
                    module.RetryEnabled && response.CanRetry && !response.OutcomeUnknown, now, module.MaxAttempts, module.GetRetryDelay(message.RetryCount + 1));
                result.FailedCount++;
            }
            log.Complete(response.IsSuccess, response.CanRetry, response.ProviderMessageId, response.Cost, response.OutcomeUnknown, now);
            await repository.CompleteAttemptAsync(message, log, cancellationToken);
        }
        return result;
    }

    private Task<SmsProviderResult> SendAsync(SmsMessage message, SmsProviderConfiguration configuration, CancellationToken cancellationToken)
    {
        var provider = providers.Resolve(configuration);
        if (message.Type == SmsType.Template || message.Type == SmsType.Otp && message.TemplateId.HasValue)
        {
            Dictionary<string, string> parameters;
            try
            {
                parameters = string.IsNullOrWhiteSpace(message.TemplateParameters) ? [] :
                    JsonSerializer.Deserialize<Dictionary<string, string>>(message.TemplateParameters) ?? [];
            }
            catch (JsonException)
            {
                return Task.FromResult(SmsProviderResult.Failure("Invalid stored SMS template parameters."));
            }
            return provider.SendTemplateAsync(message.PhoneNumber, message.TemplateId ?? 0,
                parameters, message.LineNumber, cancellationToken);
        }
        if (message.Type == SmsType.PeerToPeer)
            return provider.SendPeerToPeerAsync([new PeerToPeerSmsItem
            {
                PhoneNumber = message.PhoneNumber, Message = message.MessageBody ?? ""
            }], message.LineNumber, cancellationToken);
        if (message.Type is SmsType.Bulk or SmsType.Otp)
            return provider.SendBulkAsync([message.PhoneNumber], message.MessageBody ?? "", message.LineNumber, cancellationToken);
        return Task.FromResult(SmsProviderResult.Failure("Unsupported SMS message type."));
    }
}
