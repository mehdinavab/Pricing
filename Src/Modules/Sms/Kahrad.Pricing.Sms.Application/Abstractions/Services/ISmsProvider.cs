using Kahrad.Pricing.Sms.Application.DTOs;

namespace Kahrad.Pricing.Sms.Application.Abstractions.Services;

public interface ISmsProvider
{
    Task<SmsProviderResult> SendBulkAsync(
        IEnumerable<string> phoneNumbers,
        string message,
        string? lineNumber = null,
        CancellationToken cancellationToken = default);

    Task<SmsProviderResult> SendPeerToPeerAsync(
        IEnumerable<PeerToPeerSmsItem> items,
        string? lineNumber = null,
        CancellationToken cancellationToken = default);

    Task<SmsProviderResult> SendTemplateAsync(
        string phoneNumber,
        int templateId,
        IDictionary<string, string> parameters,
        string? lineNumber = null,
        CancellationToken cancellationToken = default);
}
