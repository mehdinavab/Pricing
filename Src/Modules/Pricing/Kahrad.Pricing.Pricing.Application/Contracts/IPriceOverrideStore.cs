using Kahrad.Pricing.Pricing.Domain.Entities;

namespace Kahrad.Pricing.Pricing.Application.Contracts;

public interface IPriceOverrideStore
{
    Task<ProductFactoryPriceOverride?> GetAsync(long brandId, long productId, CancellationToken cancellationToken);
    Task SaveAsync(ProductFactoryPriceOverride priceOverride, CancellationToken cancellationToken);
}
