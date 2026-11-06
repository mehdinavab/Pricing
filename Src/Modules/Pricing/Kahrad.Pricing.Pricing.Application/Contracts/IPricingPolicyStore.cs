using Kahrad.Pricing.Pricing.Domain.Entities;

namespace Kahrad.Pricing.Pricing.Application.Contracts;

public interface IPricingPolicyStore
{
    Task<BrandPricingPolicy?> GetBrandPolicyAsync(long brandId, CancellationToken cancellationToken);
    Task<RepresentativePricingPolicy?> GetRepresentativePolicyAsync(long brandId,
        long representativeId, CancellationToken cancellationToken);
    Task<RepresentativeProductDiscount?> GetProductDiscountAsync(long brandId,
        long representativeId, long productId, CancellationToken cancellationToken);
    Task<IReadOnlyList<RepresentativeProductDiscount>> GetProductDiscountsAsync(long brandId,
        long representativeId, CancellationToken cancellationToken);
    Task SaveAsync(BrandPricingPolicy policy, CancellationToken cancellationToken);
    Task SaveAsync(RepresentativePricingPolicy policy, CancellationToken cancellationToken);
    Task SaveAsync(RepresentativeProductDiscount discount, CancellationToken cancellationToken);
}
