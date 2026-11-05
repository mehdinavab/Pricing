using Kahrad.Pricing.BuildingBlocks.Domain.Common;

namespace Kahrad.Pricing.AccountingIntegration.Domain.Entities;

public sealed class AccountingItemMapping : BaseEntity
{
    private AccountingItemMapping() { }
    public AccountingItemMapping(long brandId, long localItemId, long accountingItemId, decimal unitConversionFactor, Guid? mappedByUserId)
    {
        if (brandId <= 0 || localItemId <= 0 || accountingItemId <= 0)
            throw new ArgumentOutOfRangeException(nameof(localItemId));
        if (unitConversionFactor <= 0) throw new ArgumentOutOfRangeException(nameof(unitConversionFactor));
        BrandId = brandId; LocalItemId = localItemId; AccountingItemId = accountingItemId;
        UnitConversionFactor = unitConversionFactor; MappedByUserId = mappedByUserId;
    }

    public long BrandId { get; private set; }
    public long LocalItemId { get; private set; }
    public long AccountingItemId { get; private set; }
    public decimal UnitConversionFactor { get; private set; }
    public Guid? MappedByUserId { get; private set; }
    public bool IsActive { get; private set; } = true;
    public void Deactivate() => IsActive = false;
}
