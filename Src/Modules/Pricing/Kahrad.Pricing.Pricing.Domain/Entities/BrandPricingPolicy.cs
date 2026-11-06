using Kahrad.Pricing.BuildingBlocks.Domain.Common;

namespace Kahrad.Pricing.Pricing.Domain.Entities;

public static class PricingPolicyDefaults
{
    public const decimal ConfirmedSellingRoundingStep = 5000m;
    public const decimal NeutralRepresentativeFactor = 1m;
}

public sealed class BrandPricingPolicy : BaseEntity
{
    private BrandPricingPolicy() { }

    public BrandPricingPolicy(long brandId, decimal beautyPercentage,
        decimal factorySalesPercentage, decimal commercePercentage,
        decimal representativeFactor = PricingPolicyDefaults.NeutralRepresentativeFactor,
        decimal factoryRoundingStep = PricingPolicyDefaults.ConfirmedSellingRoundingStep,
        Guid? actorId = null)
    {
        if (brandId <= 0) throw new ArgumentOutOfRangeException(nameof(brandId));
        BrandId = brandId;
        Change(beautyPercentage, factorySalesPercentage, commercePercentage,
            representativeFactor, factoryRoundingStep, actorId, DateTime.UtcNow);
    }

    public long BrandId { get; private set; }
    public decimal BeautyPercentage { get; private set; }
    public decimal FactorySalesPercentage { get; private set; }
    public decimal CommercePercentage { get; private set; }
    public decimal RepresentativeFactor { get; private set; }
    public decimal FactoryRoundingStep { get; private set; }

    public void Change(decimal beautyPercentage, decimal factorySalesPercentage,
        decimal commercePercentage, decimal representativeFactor, decimal factoryRoundingStep,
        Guid? actorId, DateTime nowUtc)
    {
        if (beautyPercentage < 0m || factorySalesPercentage < 0m || commercePercentage < 0m)
            throw new ArgumentOutOfRangeException(nameof(beautyPercentage),
                "Factory percentages cannot be negative.");
        if (representativeFactor <= 0m)
            throw new ArgumentOutOfRangeException(nameof(representativeFactor));
        if (factoryRoundingStep != PricingPolicyDefaults.ConfirmedSellingRoundingStep)
            throw new ArgumentOutOfRangeException(nameof(factoryRoundingStep),
                "Factory rounding step must be 5000 for the confirmed P0 policy.");

        BeautyPercentage = beautyPercentage;
        FactorySalesPercentage = factorySalesPercentage;
        CommercePercentage = commercePercentage;
        RepresentativeFactor = representativeFactor;
        FactoryRoundingStep = factoryRoundingStep;
        SetUpdated(actorId, nowUtc);
    }
}
