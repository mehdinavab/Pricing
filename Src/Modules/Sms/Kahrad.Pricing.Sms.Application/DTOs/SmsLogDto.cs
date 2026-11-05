using Kahrad.Pricing.Sms.Domain.Enums;

namespace Kahrad.Pricing.Sms.Application.DTOs;

public sealed class SmsLogDto
{
    public long Id { get; init; }
    public long SmsMessageId { get; init; }
    public long ProviderConfigurationId { get; init; }
    public int AttemptNumber { get; init; }
    public DateTime StartedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
    public SmsLogStatus Status { get; init; }
    public bool CanRetry { get; init; }
    public string? ProviderMessageId { get; init; }
    public decimal? Cost { get; init; }
    public string? ErrorCode { get; init; }
}
