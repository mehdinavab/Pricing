using Kahrad.Pricing.Pricing.Application.Contracts;
using Kahrad.Pricing.Pricing.Domain.Entities;
using Kahrad.Pricing.Products.Application.Abstractions.Persistence;

namespace Kahrad.Pricing.Pricing.Application.Services;

public sealed class ProductPriceOverrideService(IPriceOverrideStore store, IProductBrandReader products)
{
    public async Task<ProductFactoryPriceOverride> SaveAsync(long brandId, long productId,
        FactorySellingPriceMode mode, decimal? fixedPrice, decimal manualAddition,
        byte[]? rowVersion, Guid? actorId, CancellationToken ct)
    {
        if (brandId <= 0 || productId <= 0 || await products.GetBrandIdAsync(productId, ct) != brandId)
            throw new PricingOperationException("Product must belong to the selected brand.");
        if (!Enum.IsDefined(mode) || manualAddition < 0 ||
            (mode == FactorySellingPriceMode.Fixed && fixedPrice is not > 0) ||
            (mode == FactorySellingPriceMode.Calculated && fixedPrice is not null))
            throw new PricingOperationException("Factory price override is invalid.");
        var existing = await store.GetAsync(brandId, productId, ct);
        if (existing is null)
        {
            if (rowVersion is not null) throw new PricingOperationException("Price override version is invalid.", true);
            existing = new ProductFactoryPriceOverride(brandId, productId, mode, fixedPrice, manualAddition, actorId);
        }
        else
        {
            if (rowVersion is null || !existing.RowVersion.SequenceEqual(rowVersion))
                throw new PricingOperationException("Price override changed; reload before editing.", true);
            existing.Change(mode, fixedPrice, manualAddition, actorId, DateTime.UtcNow);
        }
        await store.SaveAsync(existing, ct);
        return existing;
    }

    public Task<ProductFactoryPriceOverride?> GetAsync(long brandId, long productId, CancellationToken ct) =>
        store.GetAsync(brandId, productId, ct);
}
