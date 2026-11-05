namespace Kahrad.Pricing.Sms.Infrastructure.Options;

public sealed class KavenegarOptions
{
    public const string SectionName = "Sms:Providers:Kavenegar";
    public string? ApiKey { get; set; }
    public string? LineNumber { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
    public Dictionary<int, string> Templates { get; set; } = [];
}
