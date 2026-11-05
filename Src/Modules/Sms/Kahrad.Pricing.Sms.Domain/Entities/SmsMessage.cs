using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Kahrad.Pricing.Sms.Domain.Enums;

namespace Kahrad.Pricing.Sms.Domain.Entities;









public sealed class SmsMessage : BaseEntity
{

    public Guid BatchId { get; private set; }

    public string PhoneNumber { get; private set; } = null!;

    public string? MessageBody { get; private set; }

    public SmsType Type { get; private set; }

    public SmsStatus Status { get; private set; } = SmsStatus.Pending;

    public int? TemplateId { get; private set; }

    public string? TemplateParameters { get; private set; }

    public string? LineNumber { get; private set; }

    public DateTime SendDateUtc { get; private set; }

    public DateTime ExpiresAtUtc { get; private set; }

    public DateTime? SentAtUtc { get; private set; }

    public string? ProviderMessageId { get; private set; }

    public decimal? Cost { get; private set; }

    public int RetryCount { get; private set; }

    public DateTime? NextAttemptAtUtc { get; private set; }

    public long ProviderConfigurationId { get; private set; }

    public string Provider { get; private set; } = "SmsIr";

    public void AssignProvider(SmsProviderConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        if (configuration.Id <= 0 || !configuration.IsEnabled)
            throw new InvalidOperationException("An enabled persisted SMS provider configuration is required.");
        Provider = configuration.Provider;
        ProviderConfigurationId = configuration.Id;
    }

    public string? FailureReason { get; private set; }

    private SmsMessage()
    {
    }

    public SmsMessage(
        Guid batchId,
        string phoneNumber,
        string? messageBody,
        SmsType type,
        DateTime sendDateUtc,
        int? templateId = null,
        string? templateParameters = null,
        string? lineNumber = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber);

        BatchId = batchId;
        PhoneNumber = phoneNumber.Trim();
        MessageBody = messageBody?.Trim();
        Type = type;
        TemplateId = templateId;
        TemplateParameters = templateParameters?.Trim();
        LineNumber = lineNumber?.Trim();
        SendDateUtc = sendDateUtc;
        ExpiresAtUtc = sendDateUtc.AddDays(1);
        Status = SmsStatus.Pending;
        RetryCount = 0;
    }

    public static SmsMessage CreateBulk(
        Guid batchId,
        string phoneNumber,
        string messageBody,
        DateTime sendDateUtc,
        string? lineNumber = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageBody);

        return new SmsMessage(
            batchId,
            phoneNumber,
            messageBody,
            SmsType.Bulk,
            sendDateUtc,
            lineNumber: lineNumber);
    }

    public static SmsMessage CreatePeerToPeer(
        Guid batchId,
        string phoneNumber,
        string messageBody,
        DateTime sendDateUtc,
        string? lineNumber = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageBody);

        return new SmsMessage(
            batchId,
            phoneNumber,
            messageBody,
            SmsType.PeerToPeer,
            sendDateUtc,
            lineNumber: lineNumber);
    }

    public static SmsMessage CreateTemplate(
        Guid batchId,
        string phoneNumber,
        int templateId,
        string? templateParametersJson,
        DateTime sendDateUtc,
        string? lineNumber = null)
    {
        return new SmsMessage(
            batchId,
            phoneNumber,
            messageBody: null,
            SmsType.Template,
            sendDateUtc,
            templateId: templateId,
            templateParameters: templateParametersJson,
            lineNumber: lineNumber);
    }

    public static SmsMessage CreateOtp(
        Guid batchId,
        string phoneNumber,
        string code,
        DateTime sendDateUtc,
        int? templateId = null,
        string? templateParametersJson = null,
        string? lineNumber = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        return new SmsMessage(
            batchId,
            phoneNumber,
            messageBody: $"Verification code: {code}",
            SmsType.Otp,
            sendDateUtc,
            templateId: templateId,
            templateParameters: templateParametersJson,
            lineNumber: lineNumber);
    }

    public bool IsExpired(DateTime utcNow)
    {
        return Status == SmsStatus.Pending && utcNow > ExpiresAtUtc;
    }

    public bool IsReadyToSend(DateTime utcNow)
    {
        return Status == SmsStatus.Pending && utcNow >= SendDateUtc && (!NextAttemptAtUtc.HasValue || utcNow >= NextAttemptAtUtc) && utcNow <= ExpiresAtUtc;
    }

    public void MarkAsSending(DateTime utcNow)
    {
        Status = SmsStatus.Sending;
        SetUpdated(null, utcNow);
    }

    public void MarkAsSent(string? providerMessageId, decimal? cost, DateTime utcNow)
    {
        Status = SmsStatus.Sent;
        ProviderMessageId = providerMessageId;
        Cost = cost;
        SentAtUtc = utcNow;
        FailureReason = null;
        SetUpdated(null, utcNow);
    }

    public void MarkAsFailed(string reason, bool canRetry, DateTime utcNow, int maxAttempts, TimeSpan retryDelay)
    {
        RetryCount++;
        FailureReason = reason;

        if (canRetry && RetryCount < maxAttempts && utcNow.Add(retryDelay) <= ExpiresAtUtc)
        {
            Status = SmsStatus.Pending;
            NextAttemptAtUtc = utcNow.Add(retryDelay);
        }
        else
        {
            Status = SmsStatus.Failed;
        }

        SetUpdated(null, utcNow);
    }

    public void MarkAsExpired(DateTime utcNow)
    {
        Status = SmsStatus.Expired;
        FailureReason = "SMS expired: Not sent within 24 hours of scheduled send date.";
        SetUpdated(null, utcNow);
    }
}
