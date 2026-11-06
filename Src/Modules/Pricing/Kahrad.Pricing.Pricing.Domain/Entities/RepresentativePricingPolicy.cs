using Kahrad.Pricing.BuildingBlocks.Domain.Common;

namespace Kahrad.Pricing.Pricing.Domain.Entities;

public sealed class RepresentativePricingPolicy : BaseEntity
{
    private RepresentativePricingPolicy() { }

    public RepresentativePricingPolicy(long brandId, long representativeId,
        decimal salesPercentage, decimal generalDiscountPercentage,
        decimal customerRoundingStep = PricingPolicyDefaults.ConfirmedSellingRoundingStep,
        Guid? actorId = null)
    {
        if (brandId <= 0 || representativeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(representativeId));
        BrandId = brandId;
        RepresentativeId = representativeId;
        Change(salesPercentage, generalDiscountPercentage, customerRoundingStep,
            actorId, DateTime.UtcNow);
    }

    public long BrandId { get; private set; }
    public long RepresentativeId { get; private set; }
    public decimal SalesPercentage { get; private set; }
    public decimal GeneralDiscountPercentage { get; private set; }
    public decimal CustomerRoundingStep { get; private set; }

    public void Change(decimal salesPercentage, decimal generalDiscountPercentage,
        decimal customerRoundingStep, Guid? actorId, DateTime nowUtc)
    {
        ValidatePercentage(salesPercentage, nameof(salesPercentage));
        ValidatePercentage(generalDiscountPercentage, nameof(generalDiscountPercentage));
        if (generalDiscountPercentage > salesPercentage)
            throw new ArgumentException("General discount cannot exceed representative sales percentage.");
        if (customerRoundingStep != PricingPolicyDefaults.ConfirmedSellingRoundingStep)
            throw new ArgumentOutOfRangeException(nameof(customerRoundingStep),
                "Customer rounding step must be 5000 for the confirmed P0 policy.");

        SalesPercentage = salesPercentage;
        GeneralDiscountPercentage = generalDiscountPercentage;
        CustomerRoundingStep = customerRoundingStep;
        SetUpdated(actorId, nowUtc);
    }

    private static void ValidatePercentage(decimal value, string parameter)
    {
        if (value is < 0m or > 100m)
            throw new ArgumentOutOfRangeException(parameter, "Percentage must be between 0 and 100.");
    }
}

public sealed class RepresentativeProductDiscount : BaseEntity
{
    private RepresentativeProductDiscount() { }

    public RepresentativeProductDiscount(long brandId, long representativeId,
        long productId, decimal discountPercentage, Guid? actorId = null)
    {
        if (brandId <= 0 || representativeId <= 0 || productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId));
        BrandId = brandId;
        RepresentativeId = representativeId;
        ProductId = productId;
        Change(discountPercentage, actorId, DateTime.UtcNow);
    }

    public long BrandId { get; private set; }
    public long RepresentativeId { get; private set; }
    public long ProductId { get; private set; }
    public decimal DiscountPercentage { get; private set; }

    public void Change(decimal discountPercentage, Guid? actorId, DateTime nowUtc)
    {
        if (discountPercentage is < 0m or > 100m)
            throw new ArgumentOutOfRangeException(nameof(discountPercentage),
                "Product discount must be between 0 and 100.");
        DiscountPercentage = discountPercentage;
        SetUpdated(actorId, nowUtc);
    }
}
