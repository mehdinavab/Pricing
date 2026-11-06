using Kahrad.Pricing.Pricing.Application.Contracts;
using Kahrad.Pricing.Pricing.Domain.Entities;
using Kahrad.Pricing.Products.Application.Abstractions.Persistence;
using Kahrad.Pricing.Representatives.Application.Contracts;

namespace Kahrad.Pricing.Pricing.Application.Services;

public sealed class PricingPolicyService(IPricingPolicyStore policies,
    IProductBrandReader products, IRepresentativePricingReader representatives)
{
    public async Task<BrandPricingPolicy> SaveBrandPolicyAsync(long brandId,
        decimal beautyPercentage, decimal factorySalesPercentage, decimal commercePercentage,
        decimal representativeFactor, decimal factoryRoundingStep, byte[]? rowVersion,
        Guid? actorId, CancellationToken ct)
    {
        if (brandId <= 0) throw new PricingOperationException("Brand ID must be positive.");
        var policy = await policies.GetBrandPolicyAsync(brandId, ct);
        if (policy is null)
        {
            if (rowVersion is not null)
                throw new PricingOperationException("Brand pricing policy version is invalid.", true);
            policy = new BrandPricingPolicy(brandId, beautyPercentage, factorySalesPercentage,
                commercePercentage, representativeFactor, factoryRoundingStep, actorId);
        }
        else
        {
            RequireVersion(policy.RowVersion, rowVersion);
            policy.Change(beautyPercentage, factorySalesPercentage, commercePercentage,
                representativeFactor, factoryRoundingStep, actorId, DateTime.UtcNow);
        }
        await policies.SaveAsync(policy, ct);
        return policy;
    }

    public async Task<RepresentativePricingPolicy> SaveRepresentativePolicyAsync(long brandId,
        long representativeId, decimal salesPercentage, decimal generalDiscountPercentage,
        decimal customerRoundingStep, byte[]? rowVersion, Guid? actorId, CancellationToken ct)
    {
        await RequireRepresentativeAsync(brandId, representativeId, ct);
        var conflicts = (await policies.GetProductDiscountsAsync(brandId, representativeId, ct))
            .Where(x => generalDiscountPercentage + x.DiscountPercentage > salesPercentage)
            .Select(x => x.ProductId).OrderBy(x => x).ToArray();
        if (conflicts.Length > 0)
            throw new PricingPolicyConflictException(
                "Representative policy conflicts with active product discounts.", conflicts);

        var policy = await policies.GetRepresentativePolicyAsync(brandId, representativeId, ct);
        if (policy is null)
        {
            if (rowVersion is not null)
                throw new PricingOperationException("Representative pricing policy version is invalid.", true);
            policy = new RepresentativePricingPolicy(brandId, representativeId,
                salesPercentage, generalDiscountPercentage, customerRoundingStep, actorId);
        }
        else
        {
            RequireVersion(policy.RowVersion, rowVersion);
            policy.Change(salesPercentage, generalDiscountPercentage,
                customerRoundingStep, actorId, DateTime.UtcNow);
        }
        await policies.SaveAsync(policy, ct);
        return policy;
    }

    public async Task<RepresentativeProductDiscount> SaveProductDiscountAsync(long brandId,
        long representativeId, long productId, decimal discountPercentage,
        byte[]? rowVersion, Guid? actorId, CancellationToken ct)
    {
        await RequireRepresentativeAsync(brandId, representativeId, ct);
        if (productId <= 0 || await products.GetBrandIdAsync(productId, ct) != brandId)
            throw new PricingOperationException("Product must belong to the selected brand.");
        var representativePolicy = await policies.GetRepresentativePolicyAsync(brandId,
            representativeId, ct) ?? throw new PricingOperationException(
                "Representative pricing policy is required before product discounts.");
        if (representativePolicy.GeneralDiscountPercentage + discountPercentage >
            representativePolicy.SalesPercentage)
            throw new PricingPolicyConflictException(
                "Product discount would sell below the representative purchase price.", [productId]);

        var discount = await policies.GetProductDiscountAsync(brandId, representativeId, productId, ct);
        if (discount is null)
        {
            if (rowVersion is not null)
                throw new PricingOperationException("Product discount version is invalid.", true);
            discount = new RepresentativeProductDiscount(brandId, representativeId,
                productId, discountPercentage, actorId);
        }
        else
        {
            RequireVersion(discount.RowVersion, rowVersion);
            discount.Change(discountPercentage, actorId, DateTime.UtcNow);
        }
        await policies.SaveAsync(discount, ct);
        return discount;
    }

    private async Task RequireRepresentativeAsync(long brandId, long representativeId,
        CancellationToken ct)
    {
        var representative = await representatives.GetAsync(representativeId, ct);
        if (representative is null || !representative.IsActive || representative.BrandId != brandId)
            throw new PricingOperationException("Representative must be active and belong to the selected brand.");
    }

    private static void RequireVersion(byte[] actual, byte[]? supplied)
    {
        if (supplied is null || !actual.SequenceEqual(supplied))
            throw new PricingOperationException("Pricing policy changed; reload before editing.", true);
    }
}
