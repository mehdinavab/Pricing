namespace Kahrad.Pricing.Sms.Application.DTOs;

public class SmsBatchResponse
{
    public Guid BatchId { get; init; }
    public int TotalQueued { get; init; }
    public DateTime ScheduledSendDateUtc { get; init; }
    public string Status { get; init; } = "Pending";
}
