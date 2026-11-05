namespace Kahrad.Pricing.Sms.Infrastructure.Options;

public sealed class SmsIrOptions
{
    public const string SectionName = "SmsIr";

    public int TimeoutSeconds { get; set; } = 30;
    public string? ApiKey { get; set; }
    public string? LineNumber { get; set; }
}
