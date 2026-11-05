using Kahrad.Pricing.BuildingBlocks.Domain.Common;

namespace Kahrad.Pricing.Sales.Domain.Entities;

public enum AccountingSalePostStatus { Pending, InProgress, Succeeded, RetryScheduled, Failed, RequiresManualReview }

public sealed class AccountingSalePostOutbox : BaseEntity
{
    private AccountingSalePostOutbox() { }
    public AccountingSalePostOutbox(SoldProductUnit soldUnit)
    {
        ArgumentNullException.ThrowIfNull(soldUnit);
        SoldProductUnitPublicId = soldUnit.PublicId;
        BrandId = soldUnit.BrandId;
        IdempotencyKey = $"sale-price:{soldUnit.PublicId:N}:v1";
        NextAttemptAtUtc = DateTime.UtcNow;
    }

    public Guid SoldProductUnitPublicId { get; private set; }
    public long BrandId { get; private set; }
    public string IdempotencyKey { get; private set; } = null!;
    public AccountingSalePostStatus Status { get; private set; } = AccountingSalePostStatus.Pending;
    public int AttemptCount { get; private set; }
    public DateTime? NextAttemptAtUtc { get; private set; }
    public DateTime? LastAttemptAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public string? ExternalReference { get; private set; }
    public string? ErrorCategory { get; private set; }

    public void StartAttempt(DateTime nowUtc)
    {
        if (Status is not (AccountingSalePostStatus.Pending or AccountingSalePostStatus.RetryScheduled))
            throw new InvalidOperationException("Sale posting is not eligible for an attempt.");
        Status = AccountingSalePostStatus.InProgress; AttemptCount++;
        LastAttemptAtUtc = nowUtc; NextAttemptAtUtc = null;
    }

    public void Succeed(string externalReference, DateTime nowUtc)
    {
        if (Status != AccountingSalePostStatus.InProgress)
            throw new InvalidOperationException("Sale posting is not in progress.");
        ArgumentException.ThrowIfNullOrWhiteSpace(externalReference);
        ExternalReference = externalReference.Trim(); Status = AccountingSalePostStatus.Succeeded;
        CompletedAtUtc = nowUtc; ErrorCategory = null;
    }

    public void Fail(string category, bool outcomeUnknown, bool retrySafe, DateTime nowUtc,
        DateTime? nextAttemptAtUtc, int maxAttempts)
    {
        if (Status != AccountingSalePostStatus.InProgress)
            throw new InvalidOperationException("Sale posting is not in progress.");
        ArgumentException.ThrowIfNullOrWhiteSpace(category);
        ErrorCategory = category.Length <= 100 ? category : category[..100];
        if (outcomeUnknown)
        {
            Status = AccountingSalePostStatus.RequiresManualReview;
            CompletedAtUtc = nowUtc;
        }
        else if (retrySafe && AttemptCount < maxAttempts && nextAttemptAtUtc > nowUtc)
        {
            Status = AccountingSalePostStatus.RetryScheduled;
            NextAttemptAtUtc = nextAttemptAtUtc;
        }
        else
        {
            Status = AccountingSalePostStatus.Failed;
            CompletedAtUtc = nowUtc;
        }
    }
}
