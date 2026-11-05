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
        cancellationToken.ThrowIfCancellationRequested();
        var numbers = phoneNumbers.ToArray();
        if (numbers.Length == 0)
            return SmsProviderResult.Failure("No phone numbers provided.");

        var line = ResolveLineNumber(lineNumber);

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            return SmsProviderResult.Failure("SMS.ir API key is not configured.");
        }

        try
        {
            var client = new SmsIr(_options.ApiKey);
            var response = await client.BulkSendAsync(line, message, numbers).WaitAsync(TimeSpan.FromSeconds(_options.TimeoutSeconds), cancellationToken);

            if (response?.Data != null)
            {
                return SmsProviderResult.Success(
                    response.Data.PackId.ToString(),
                    (decimal?)response.Data.Cost);
            }

            return SmsProviderResult.Failure("SMS.ir returned no receipt; investigate delivery before resending.", outcomeUnknown: true);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (SmsIrException)
        {
            logger.LogWarning("SMS.ir request failed; delivery requires investigation.");
            return SmsProviderResult.Failure("SMS.ir request failed; delivery outcome may be unknown.", outcomeUnknown: true);
        }
        catch (Exception)
        {
            logger.LogWarning("SMS.ir request failed; delivery requires investigation.");
            return SmsProviderResult.Failure("SMS.ir request failed; delivery outcome may be unknown.", outcomeUnknown: true);
        }
    }

    public async Task<SmsProviderResult> SendPeerToPeerAsync(
        IEnumerable<PeerToPeerSmsItem> items,
        string? lineNumber = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var itemList = items.ToList();
        if (itemList.Count == 0)
            return SmsProviderResult.Failure("No items provided.");

        var numbers = itemList.Select(x => x.PhoneNumber).ToArray();
        var messages = itemList.Select(x => x.Message).ToArray();
        var line = ResolveLineNumber(lineNumber);

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            return SmsProviderResult.Failure("SMS.ir API key is not configured.");
        }

        try
        {
            var client = new SmsIr(_options.ApiKey);
            var response = await client.LikeToLikeSendAsync(line, numbers, messages).WaitAsync(TimeSpan.FromSeconds(_options.TimeoutSeconds), cancellationToken);

            if (response?.Data != null)
            {
                return SmsProviderResult.Success(
                    response.Data.PackId.ToString(),
                    (decimal?)response.Data.Cost);
            }

            return SmsProviderResult.Failure("SMS.ir returned no receipt; investigate delivery before resending.", outcomeUnknown: true);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (SmsIrException)
        {
            logger.LogWarning("SMS.ir request failed; delivery requires investigation.");
            return SmsProviderResult.Failure("SMS.ir request failed; delivery outcome may be unknown.", outcomeUnknown: true);
        }
        catch (Exception)
        {
            logger.LogWarning("SMS.ir request failed; delivery requires investigation.");
            return SmsProviderResult.Failure("SMS.ir request failed; delivery outcome may be unknown.", outcomeUnknown: true);
        }
    }

    public async Task<SmsProviderResult> SendTemplateAsync(
        string phoneNumber,
        int templateId,
        IDictionary<string, string> parameters,
        string? lineNumber = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            return SmsProviderResult.Failure("SMS.ir API key is not configured.");
        }

        try
        {
            var client = new SmsIr(_options.ApiKey);
            var templateParams = parameters.Select(kv => new VerifySendParameter(kv.Key, kv.Value)).ToArray();
            var response = await client.VerifySendAsync(phoneNumber, templateId, templateParams).WaitAsync(TimeSpan.FromSeconds(_options.TimeoutSeconds), cancellationToken);

            if (response?.Data != null)
            {
                return SmsProviderResult.Success(
                    response.Data.MessageId.ToString(),
                    (decimal?)response.Data.Cost);
            }

            return SmsProviderResult.Failure("SMS.ir returned no receipt; investigate delivery before resending.", outcomeUnknown: true);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (SmsIrException)
        {
            logger.LogWarning("SMS.ir request failed; delivery requires investigation.");
            return SmsProviderResult.Failure("SMS.ir request failed; delivery outcome may be unknown.", outcomeUnknown: true);
        }
        catch (Exception)
        {
            logger.LogWarning("SMS.ir request failed; delivery requires investigation.");
            return SmsProviderResult.Failure("SMS.ir request failed; delivery outcome may be unknown.", outcomeUnknown: true);
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
