using Kahrad.Pricing.Sms.Domain.Enums;

namespace Kahrad.Pricing.Sms.Application.DTOs;

public class SmsMessageDto
{
    public long Id { get; init; }
    public Guid PublicId { get; init; }
    public Guid BatchId { get; init; }
    public string PhoneNumber { get; init; } = null!;
    public string? MessageBody { get; init; }
    public SmsType Type { get; init; }
    public SmsStatus Status { get; init; }
    public int? TemplateId { get; init; }
    public string? TemplateParameters { get; init; }
    public string? LineNumber { get; init; }
    public DateTime SendDateUtc { get; init; }
    public DateTime ExpiresAtUtc { get; init; }
    public DateTime? SentAtUtc { get; init; }
    public string? ProviderMessageId { get; init; }
    public decimal? Cost { get; init; }
    public long ProviderConfigurationId { get; init; }
    public string Provider { get; init; } = null!;
    public DateTime? NextAttemptAtUtc { get; init; }
    public int RetryCount { get; init; }
    public string? FailureReason { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
