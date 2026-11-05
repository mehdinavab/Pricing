using Kahrad.Pricing.BuildingBlocks.Domain.Common;

namespace Kahrad.Pricing.Pricing.Domain.Entities;

public enum FactorySellingPriceMode { Calculated, Fixed }

/// <summary>Stores a brand-scoped policy choice; it does not itself calculate or publish prices.</summary>
public sealed class ProductFactoryPriceOverride : BaseEntity
{
    private ProductFactoryPriceOverride() { }
    public ProductFactoryPriceOverride(long brandId, long productId, FactorySellingPriceMode mode,
        decimal? fixedPrice, decimal manualAddition, Guid? actorId)
    {
        if (brandId <= 0 || productId <= 0) throw new ArgumentOutOfRangeException(nameof(productId));
        BrandId = brandId; ProductId = productId;
        Change(mode, fixedPrice, manualAddition, actorId, DateTime.UtcNow);
    }

    public long BrandId { get; private set; }
    public long ProductId { get; private set; }
    public FactorySellingPriceMode Mode { get; private set; }
    public decimal? FixedFactorySellingPrice { get; private set; }
    public decimal ManualFactoryPriceAdjustment { get; private set; }

    public void Change(FactorySellingPriceMode mode, decimal? fixedPrice, decimal manualAddition,
        Guid? actorId, DateTime nowUtc)
    {
        if (!Enum.IsDefined(mode) || manualAddition < 0 ||
            (mode == FactorySellingPriceMode.Fixed && fixedPrice is not > 0) ||
            (mode == FactorySellingPriceMode.Calculated && fixedPrice is not null))
            throw new ArgumentException("Factory price override is invalid.");
        Mode = mode; FixedFactorySellingPrice = fixedPrice;
        ManualFactoryPriceAdjustment = manualAddition;
        SetUpdated(actorId, nowUtc);
    }
}
