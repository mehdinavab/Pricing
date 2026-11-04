namespace Kahrad.Pricing.Sms.Application.DTOs;

public class SendOtpSmsRequest
{
    public string PhoneNumber { get; init; } = null!;
    public string Code { get; init; } = null!;
    public int? TemplateId { get; init; }
    public Dictionary<string, string>? Parameters { get; init; }
    public string? LineNumber { get; init; }
}
