using Kahrad.Pricing.Sms.Application.Abstractions.Persistence;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.DTOs;
using Kahrad.Pricing.Sms.Domain.Entities;
using Kahrad.Pricing.Sms.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Kahrad.Pricing.Sms.Infrastructure.Services;

public sealed class SmsOutboxProcessor(
    ISmsMessageRepository smsMessageRepository,
    ISmsProvider smsProvider,
    ILogger<SmsOutboxProcessor> logger) : ISmsOutboxProcessor
{
    public async Task<SmsOutboxProcessResult> ProcessPendingOutboxAsync(
        CancellationToken cancellationToken = default)
    {
        var result = new SmsOutboxProcessResult();
        var utcNow = DateTime.UtcNow;

        // 1. Process Expired Messages (not sent within 24 hours of scheduled send date)
        await ProcessExpiredMessagesAsync(utcNow, result, cancellationToken);

        // 2. Process Ready Pending Messages
        await ProcessReadyMessagesAsync(utcNow, result, cancellationToken);

        return result;
    }

    private async Task ProcessExpiredMessagesAsync(
        DateTime utcNow,
        SmsOutboxProcessResult result,
        CancellationToken cancellationToken)
    {
        var expiredMessages = await smsMessageRepository.GetExpiredPendingMessagesAsync(utcNow, cancellationToken);
        if (expiredMessages.Count == 0)
            return;

        logger.LogInformation("Found {Count} expired SMS messages to mark.", expiredMessages.Count);

        foreach (var msg in expiredMessages)
        {
            msg.MarkAsExpired(utcNow);
        }

        await smsMessageRepository.UpdateRangeAsync(expiredMessages, cancellationToken);
        result.ExpiredCount = expiredMessages.Count;
    }

    private async Task ProcessReadyMessagesAsync(
        DateTime utcNow,
        SmsOutboxProcessResult result,
        CancellationToken cancellationToken)
    {
        var pendingMessages = await smsMessageRepository.GetPendingMessagesAsync(100, utcNow, cancellationToken);
        if (pendingMessages.Count == 0)
            return;

        logger.LogInformation("Processing {Count} pending SMS messages ready to send.", pendingMessages.Count);

        var batchGroups = pendingMessages.GroupBy(x => x.BatchId);

        foreach (var batch in batchGroups)
        {
            var messages = batch.ToList();
            var first = messages[0];

            switch (first.Type)
            {
                case SmsType.Bulk:
                    await ProcessBulkBatchAsync(messages, result, cancellationToken);
                    break;

                case SmsType.PeerToPeer:
                    await ProcessPeerToPeerBatchAsync(messages, result, cancellationToken);
                    break;

                case SmsType.Template:
                    await ProcessTemplateBatchAsync(messages, result, cancellationToken);
                    break;

                case SmsType.Otp:
                    await ProcessOtpBatchAsync(messages, result, cancellationToken);
                    break;
            }
        }
    }

    private async Task ProcessBulkBatchAsync(
        List<SmsMessage> messages,
        SmsOutboxProcessResult result,
        CancellationToken cancellationToken)
    {
        var first = messages[0];
        var body = first.MessageBody ?? string.Empty;
        var phoneNumbers = messages.Select(x => x.PhoneNumber).ToList();
        var utcNow = DateTime.UtcNow;

        foreach (var msg in messages)
        {
            msg.MarkAsSending(utcNow);
        }
        await smsMessageRepository.UpdateRangeAsync(messages, cancellationToken);

        var providerResult = await smsProvider.SendBulkAsync(phoneNumbers, body, first.LineNumber, cancellationToken);
        utcNow = DateTime.UtcNow;

        if (providerResult.IsSuccess)
        {
            foreach (var msg in messages)
            {
                msg.MarkAsSent(providerResult.ProviderMessageId, providerResult.Cost, utcNow);
            }
            result.SentCount += messages.Count;
        }
        else
        {
            foreach (var msg in messages)
            {
                msg.MarkAsFailed(providerResult.ErrorMessage ?? "Provider error", canRetry: true, utcNow);
            }
            result.FailedCount += messages.Count;
        }

        await smsMessageRepository.UpdateRangeAsync(messages, cancellationToken);
    }

    private async Task ProcessPeerToPeerBatchAsync(
        List<SmsMessage> messages,
        SmsOutboxProcessResult result,
        CancellationToken cancellationToken)
    {
        var first = messages[0];
        var items = messages.Select(m => new PeerToPeerSmsItem
        {
            PhoneNumber = m.PhoneNumber,
            Message = m.MessageBody ?? string.Empty
        }).ToList();
        var utcNow = DateTime.UtcNow;

        foreach (var msg in messages)
        {
            msg.MarkAsSending(utcNow);
        }
        await smsMessageRepository.UpdateRangeAsync(messages, cancellationToken);

        var providerResult = await smsProvider.SendPeerToPeerAsync(items, first.LineNumber, cancellationToken);
        utcNow = DateTime.UtcNow;

        if (providerResult.IsSuccess)
        {
            foreach (var msg in messages)
            {
                msg.MarkAsSent(providerResult.ProviderMessageId, providerResult.Cost, utcNow);
            }
            result.SentCount += messages.Count;
        }
        else
        {
            foreach (var msg in messages)
            {
                msg.MarkAsFailed(providerResult.ErrorMessage ?? "Provider error", canRetry: true, utcNow);
            }
            result.FailedCount += messages.Count;
        }

        await smsMessageRepository.UpdateRangeAsync(messages, cancellationToken);
    }

    private async Task ProcessTemplateBatchAsync(
        List<SmsMessage> messages,
        SmsOutboxProcessResult result,
        CancellationToken cancellationToken)
    {
        foreach (var msg in messages)
        {
            var utcNow = DateTime.UtcNow;
            msg.MarkAsSending(utcNow);
            await smsMessageRepository.UpdateAsync(msg, cancellationToken);

            var parameters = DeserializeParameters(msg.TemplateParameters);
            var providerResult = await smsProvider.SendTemplateAsync(
                msg.PhoneNumber,
                msg.TemplateId ?? 0,
                parameters,
                msg.LineNumber,
                cancellationToken);

            utcNow = DateTime.UtcNow;
            if (providerResult.IsSuccess)
            {
                msg.MarkAsSent(providerResult.ProviderMessageId, providerResult.Cost, utcNow);
                result.SentCount++;
            }
            else
            {
                msg.MarkAsFailed(providerResult.ErrorMessage ?? "Template send failed", canRetry: true, utcNow);
                result.FailedCount++;
            }

            await smsMessageRepository.UpdateAsync(msg, cancellationToken);
        }
    }

    private async Task ProcessOtpBatchAsync(
        List<SmsMessage> messages,
        SmsOutboxProcessResult result,
        CancellationToken cancellationToken)
    {
        foreach (var msg in messages)
        {
            var utcNow = DateTime.UtcNow;
            msg.MarkAsSending(utcNow);
            await smsMessageRepository.UpdateAsync(msg, cancellationToken);

            SmsProviderResult providerResult;

            if (msg.TemplateId.HasValue && msg.TemplateId.Value > 0)
            {
                var parameters = DeserializeParameters(msg.TemplateParameters);
                providerResult = await smsProvider.SendTemplateAsync(
                    msg.PhoneNumber,
                    msg.TemplateId.Value,
                    parameters,
                    msg.LineNumber,
                    cancellationToken);
            }
            else
            {
                providerResult = await smsProvider.SendBulkAsync(
                    [msg.PhoneNumber],
                    msg.MessageBody ?? string.Empty,
                    msg.LineNumber,
                    cancellationToken);
            }

            utcNow = DateTime.UtcNow;
            if (providerResult.IsSuccess)
            {
                msg.MarkAsSent(providerResult.ProviderMessageId, providerResult.Cost, utcNow);
                result.SentCount++;
            }
            else
            {
                msg.MarkAsFailed(providerResult.ErrorMessage ?? "OTP send failed", canRetry: true, utcNow);
                result.FailedCount++;
            }

            await smsMessageRepository.UpdateAsync(msg, cancellationToken);
        }
    }

    private static Dictionary<string, string> DeserializeParameters(string? parametersJson)
    {
        if (string.IsNullOrWhiteSpace(parametersJson))
            return [];

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(parametersJson) ?? [];
        }
        catch
        {
            return [];
        }
    }
}
