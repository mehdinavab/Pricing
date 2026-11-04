using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Kahrad.Pricing.Sms.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kahrad.Pricing.Sms.Domain.Entities;

[Table("Messages", Schema = "sms")]
[Index(nameof(PublicId), IsUnique = true)]
[Index(nameof(BatchId))]
[Index(nameof(Status))]
[Index(nameof(Type))]
[Index(nameof(SendDateUtc))]
[Index(nameof(ExpiresAtUtc))]
[Index(nameof(PhoneNumber))]
[Index(nameof(IsDeleted))]
public sealed class SmsMessage : BaseEntity
{
    [Sieve(CanFilter = true, CanSort = true)]
    public Guid BatchId { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string PhoneNumber { get; private set; } = null!;

    public string? MessageBody { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public SmsType Type { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public SmsStatus Status { get; private set; } = SmsStatus.Pending;

    [Sieve(CanFilter = true, CanSort = true)]
    public int? TemplateId { get; private set; }

    public string? TemplateParameters { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string? LineNumber { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime SendDateUtc { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime ExpiresAtUtc { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime? SentAtUtc { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string? ProviderMessageId { get; private set; }

    public decimal? Cost { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public int RetryCount { get; private set; }

    public int MaxRetries { get; private set; } = 3;

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
        string? lineNumber = null,
        int maxRetries = 3)
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
        MaxRetries = maxRetries;
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
        return Status == SmsStatus.Pending && utcNow >= SendDateUtc && utcNow <= ExpiresAtUtc;
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

    public void MarkAsFailed(string reason, bool canRetry, DateTime utcNow)
    {
        RetryCount++;
        FailureReason = reason;

        if (canRetry && RetryCount < MaxRetries && utcNow <= ExpiresAtUtc)
        {
            Status = SmsStatus.Pending; // Available for next retry attempt
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
