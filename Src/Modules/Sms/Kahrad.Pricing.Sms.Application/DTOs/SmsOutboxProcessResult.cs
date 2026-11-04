namespace Kahrad.Pricing.Sms.Application.DTOs;

public class SmsOutboxProcessResult
{
    public int ExpiredCount { get; set; }
    public int SentCount { get; set; }
    public int FailedCount { get; set; }
    public int ProcessedCount => ExpiredCount + SentCount + FailedCount;
}
