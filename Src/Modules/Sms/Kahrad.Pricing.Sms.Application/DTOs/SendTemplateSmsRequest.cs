namespace Kahrad.Pricing.Sms.Application.DTOs;

public class SendTemplateSmsRequest
{
    public string PhoneNumber { get; init; } = null!;
    public int TemplateId { get; init; }
    public Dictionary<string, string> Parameters { get; init; } = [];
    public DateTime? SendDateUtc { get; init; }
    public string? LineNumber { get; init; }
}
