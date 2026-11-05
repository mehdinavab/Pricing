using Kahrad.Pricing.BuildingBlocks.Domain.Common;

namespace Kahrad.Pricing.AccountingIntegration.Domain.Entities;

public sealed class AccountingItemPrice : BaseEntity
{
    private AccountingItemPrice() { }
    public AccountingItemPrice(long brandId, long accountingItemId, decimal amount, string currencyCode, long sourceSyncRunId, DateTime observedAtUtc)
    {
        if (brandId <= 0 || accountingItemId <= 0 || sourceSyncRunId <= 0)
            throw new ArgumentOutOfRangeException(nameof(accountingItemId));
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);
        BrandId = brandId; AccountingItemId = accountingItemId; Amount = amount;
        CurrencyCode = currencyCode.Trim().ToUpperInvariant();
        SourceSyncRunId = sourceSyncRunId; ObservedAtUtc = observedAtUtc;
    }

    public long BrandId { get; private set; }
    public long AccountingItemId { get; private set; }
    public decimal Amount { get; private set; }
    public string CurrencyCode { get; private set; } = null!;
    public long SourceSyncRunId { get; private set; }
    public DateTime ObservedAtUtc { get; private set; }
}
