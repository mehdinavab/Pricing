using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Kahrad.Pricing.Sms.Domain.Enums;

namespace Kahrad.Pricing.Sms.Domain.Entities;

public sealed class SmsLog : BaseEntity
{
    public long SmsMessageId { get; private set; }
    public long ProviderConfigurationId { get; private set; }
    public int AttemptNumber { get; private set; }
    public DateTime StartedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public SmsLogStatus Status { get; private set; }
    public bool CanRetry { get; private set; }
    public string? ProviderMessageId { get; private set; }
    public decimal? Cost { get; private set; }
    public string? ErrorCode { get; private set; }

    private SmsLog() { }

    public SmsLog(long smsMessageId, long providerConfigurationId, int attemptNumber, DateTime startedAtUtc)
    {
        if (smsMessageId <= 0) throw new ArgumentOutOfRangeException(nameof(smsMessageId));
        if (providerConfigurationId <= 0) throw new ArgumentOutOfRangeException(nameof(providerConfigurationId));
        if (attemptNumber <= 0) throw new ArgumentOutOfRangeException(nameof(attemptNumber));
        SmsMessageId = smsMessageId;
        ProviderConfigurationId = providerConfigurationId;
        AttemptNumber = attemptNumber;
        StartedAtUtc = startedAtUtc;
        Status = SmsLogStatus.Started;
    }

    public void Complete(bool success, bool canRetry, string? providerMessageId, decimal? cost,
        bool outcomeUnknown, DateTime completedAtUtc)
    {
        if (Status != SmsLogStatus.Started) throw new InvalidOperationException("SMS log has already completed.");
        if (completedAtUtc < StartedAtUtc) throw new ArgumentOutOfRangeException(nameof(completedAtUtc));
        if (providerMessageId?.Length > 100) throw new ArgumentException("Provider receipt is too long.", nameof(providerMessageId));
        Status = success ? SmsLogStatus.Succeeded : outcomeUnknown ? SmsLogStatus.Unknown : SmsLogStatus.Failed;
        CanRetry = !success && !outcomeUnknown && canRetry;
        ProviderMessageId = providerMessageId;
        Cost = cost;
        ErrorCode = success ? null : outcomeUnknown ? "DeliveryOutcomeUnknown" : "ProviderRejected";
        CompletedAtUtc = completedAtUtc;
    }
}
