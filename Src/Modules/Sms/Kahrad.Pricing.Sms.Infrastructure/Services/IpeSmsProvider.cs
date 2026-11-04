using IPE.SmsIrClient;
using IPE.SmsIrClient.Exceptions;
using IPE.SmsIrClient.Models.Requests;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.DTOs;
using Kahrad.Pricing.Sms.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kahrad.Pricing.Sms.Infrastructure.Services;

public sealed class IpeSmsProvider(
    IOptions<SmsIrOptions> options,
    ILogger<IpeSmsProvider> logger) : ISmsProvider
{
    private readonly SmsIrOptions _options = options.Value;

    public async Task<SmsProviderResult> SendBulkAsync(
        IEnumerable<string> phoneNumbers,
        string message,
        string? lineNumber = null,
        CancellationToken cancellationToken = default)
    {
        var numbers = phoneNumbers.ToArray();
        if (numbers.Length == 0)
            return SmsProviderResult.Failure("No phone numbers provided.");

        var line = ResolveLineNumber(lineNumber);

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            logger.LogWarning("SMS.ir API key is not configured. Simulating bulk send to {Count} numbers.", numbers.Length);
            return SmsProviderResult.Success($"simulated-bulk-{Guid.NewGuid()}", 0);
        }

        try
        {
            var client = new SmsIr(_options.ApiKey);
            var response = await client.BulkSendAsync(line, message, numbers);

            if (response?.Data != null)
            {
                return SmsProviderResult.Success(
                    response.Data.PackId.ToString(),
                    (decimal?)response.Data.Cost);
            }

            return SmsProviderResult.Failure(response?.Message ?? "Bulk send failed on provider.");
        }
        catch (SmsIrException ex)
        {
            logger.LogError(ex, "SMS.ir bulk send failed with status {Status}: {Message}", ex.Status, ex.Message);
            return SmsProviderResult.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred during SMS.ir bulk send.");
            return SmsProviderResult.Failure(ex.Message);
        }
    }

    public async Task<SmsProviderResult> SendPeerToPeerAsync(
        IEnumerable<PeerToPeerSmsItem> items,
        string? lineNumber = null,
        CancellationToken cancellationToken = default)
    {
        var itemList = items.ToList();
        if (itemList.Count == 0)
            return SmsProviderResult.Failure("No items provided.");

        var numbers = itemList.Select(x => x.PhoneNumber).ToArray();
        var messages = itemList.Select(x => x.Message).ToArray();
        var line = ResolveLineNumber(lineNumber);

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            logger.LogWarning("SMS.ir API key is not configured. Simulating peer-to-peer send to {Count} recipients.", numbers.Length);
            return SmsProviderResult.Success($"simulated-p2p-{Guid.NewGuid()}", 0);
        }

        try
        {
            var client = new SmsIr(_options.ApiKey);
            var response = await client.LikeToLikeSendAsync(line, numbers, messages);

            if (response?.Data != null)
            {
                return SmsProviderResult.Success(
                    response.Data.PackId.ToString(),
                    (decimal?)response.Data.Cost);
            }

            return SmsProviderResult.Failure(response?.Message ?? "Peer-to-peer send failed on provider.");
        }
        catch (SmsIrException ex)
        {
            logger.LogError(ex, "SMS.ir like-to-like send failed with status {Status}: {Message}", ex.Status, ex.Message);
            return SmsProviderResult.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred during SMS.ir like-to-like send.");
            return SmsProviderResult.Failure(ex.Message);
        }
    }

    public async Task<SmsProviderResult> SendTemplateAsync(
        string phoneNumber,
        int templateId,
        IDictionary<string, string> parameters,
        string? lineNumber = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            logger.LogWarning(
                "SMS.ir API key is not configured. Simulating template send (TemplateId: {TemplateId}) to {PhoneNumber}.",
                templateId,
                phoneNumber);
            return SmsProviderResult.Success($"simulated-template-{Guid.NewGuid()}", 0);
        }

        try
        {
            var client = new SmsIr(_options.ApiKey);
            var templateParams = parameters.Select(kv => new VerifySendParameter(kv.Key, kv.Value)).ToArray();
            var response = await client.VerifySendAsync(phoneNumber, templateId, templateParams);

            if (response?.Data != null)
            {
                return SmsProviderResult.Success(
                    response.Data.MessageId.ToString(),
                    (decimal?)response.Data.Cost);
            }

            return SmsProviderResult.Failure(response?.Message ?? "Template send failed on provider.");
        }
        catch (SmsIrException ex)
        {
            logger.LogError(ex, "SMS.ir verify send failed with status {Status}: {Message}", ex.Status, ex.Message);
            return SmsProviderResult.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred during SMS.ir verify send.");
            return SmsProviderResult.Failure(ex.Message);
        }
    }

    private long ResolveLineNumber(string? overrideLine)
    {
        var rawLine = !string.IsNullOrWhiteSpace(overrideLine)
            ? overrideLine
            : _options.LineNumber;

        return long.TryParse(rawLine, out var parsed) ? parsed : 300000;
    }
}
