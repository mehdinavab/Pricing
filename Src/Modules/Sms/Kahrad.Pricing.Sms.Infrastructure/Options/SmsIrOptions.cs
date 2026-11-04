namespace Kahrad.Pricing.Sms.Infrastructure.Options;

public sealed class SmsIrOptions
{
    public const string SectionName = "SmsIr";

    public string? ApiKey { get; set; }
    public string? LineNumber { get; set; }
}
