namespace Kahrad.Pricing.Sms.Application.DTOs;

public class SendBulkSmsRequest
{
    public List<string> PhoneNumbers { get; init; } = [];
    public string Message { get; init; } = null!;
    public DateTime? SendDateUtc { get; init; }
    public string? LineNumber { get; init; }
}
