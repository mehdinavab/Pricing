using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Kahrad.Pricing.Pricing.Application;
using Kahrad.Pricing.Pricing.Application.Contracts;
using Kahrad.Pricing.Pricing.Application.Services;
using Kahrad.Pricing.Pricing.Domain.Entities;
using Kahrad.Pricing.Pricing.Engine;
using Kahrad.Pricing.Products.Application.Abstractions.Persistence;
using Kahrad.Pricing.Representatives.Application.Contracts;

namespace Kahrad.Pricing.Pricing.Tests;

public sealed class PricingPolicyTests
{
    [Fact]
    public async Task General_discount_update_reports_conflicting_product_ids()
    {
        var store = new PolicyStore
        {
            Discounts =
            [
                new RepresentativeProductDiscount(1, 10, 200, 6m),
                new RepresentativeProductDiscount(1, 10, 100, 3m)
            ]
        };
        var service = new PricingPolicyService(store, new ProductReader(1),
            new RepresentativeReader(new(10, 1, true)));

        var exception = await Assert.ThrowsAsync<PricingPolicyConflictException>(() =>
            service.SaveRepresentativePolicyAsync(1, 10, 10m, 5m, 5000m,
                null, null, default));
        Assert.Equal([200L], exception.AffectedProductIds);
    }

    [Fact]
    public async Task Product_discount_enforces_product_and_representative_brand()
    {
        var store = new PolicyStore
        {
            RepresentativePolicy = new RepresentativePricingPolicy(1, 10, 20m, 5m)
        };
        var service = new PricingPolicyService(store, new ProductReader(2),
            new RepresentativeReader(new(10, 1, true)));

        await Assert.ThrowsAsync<PricingOperationException>(() =>
            service.SaveProductDiscountAsync(1, 10, 100, 3m, null, null, default));
    }

    [Fact]
    public async Task Preview_resolves_existing_models_and_does_not_write_sales()
    {
        var policies = new PolicyStore
        {
            BrandPolicy = new BrandPricingPolicy(1, 10m, 15m, 5m),
            RepresentativePolicy = new RepresentativePricingPolicy(1, 10, 20m, 5m),
            Discounts = [new RepresentativeProductDiscount(1, 10, 100, 3m)]
        };
        var service = new ProductPricePreviewService(
            new CompositionReader(new(100, 1,
                [new ProductPricingComponent(20, 2m, "EA"),
                 new ProductPricingComponent(21, 1m, "EA")])),
            new EffectivePriceReader(
                [new(20, 1, 100000m, "IRR", 1m, 7),
                 new(21, 1, 150000m, "IRR", 1m, 7)]),
            new RepresentativeReader(new(10, 1, true)), policies,
            new OverrideStore());

        var result = await service.CalculateAsync(100, 10, 50000m, default);
        Assert.Equal(350000m, result.ProductionCost);
        Assert.All(result.Components, component => Assert.Equal(7, component.SourcePriceVersion));
        Assert.Equal(505000m, result.RepresentativePrice);
        Assert.Equal(570000m, result.CustomerSellingPrice);
        Assert.Equal(0, policies.SaveCount);
    }

    [Fact]
    public async Task Preview_rejects_missing_effective_item_price_without_cross_brand_fallback()
    {
        var policies = new PolicyStore
        {
            BrandPolicy = new BrandPricingPolicy(1, 0m, 0m, 0m),
            RepresentativePolicy = new RepresentativePricingPolicy(1, 10, 0m, 0m)
        };
        var service = new ProductPricePreviewService(
            new CompositionReader(new(100, 1, [new ProductPricingComponent(20, 1m, "EA")])),
            new EffectivePriceReader([new(20, 2, 100m, "IRR", 1m, 1)]),
            new RepresentativeReader(new(10, 1, true)), policies, new OverrideStore());

        var exception = await Assert.ThrowsAsync<PricingCalculationException>(() =>
            service.CalculateAsync(100, 10, 0m, default));
        Assert.Equal(PricingCalculationErrorCode.ItemPriceMissing, exception.ErrorCode);
        Assert.Equal(20, exception.ItemId);
    }

    private sealed class PolicyStore : IPricingPolicyStore
    {
        public BrandPricingPolicy? BrandPolicy { get; init; }
        public RepresentativePricingPolicy? RepresentativePolicy { get; init; }
        public IReadOnlyList<RepresentativeProductDiscount> Discounts { get; init; } = [];
        public int SaveCount { get; private set; }
        public Task<BrandPricingPolicy?> GetBrandPolicyAsync(long brandId, CancellationToken ct) =>
            Task.FromResult(BrandPolicy?.BrandId == brandId ? BrandPolicy : null);
        public Task<RepresentativePricingPolicy?> GetRepresentativePolicyAsync(long brandId,
            long representativeId, CancellationToken ct) => Task.FromResult(
            RepresentativePolicy is { } p && p.BrandId == brandId && p.RepresentativeId == representativeId
                ? p : null);
        public Task<RepresentativeProductDiscount?> GetProductDiscountAsync(long brandId,
            long representativeId, long productId, CancellationToken ct) => Task.FromResult(
            Discounts.SingleOrDefault(x => x.BrandId == brandId &&
                x.RepresentativeId == representativeId && x.ProductId == productId));
        public Task<IReadOnlyList<RepresentativeProductDiscount>> GetProductDiscountsAsync(long brandId,
            long representativeId, CancellationToken ct) => Task.FromResult<IReadOnlyList<RepresentativeProductDiscount>>(
            Discounts.Where(x => x.BrandId == brandId && x.RepresentativeId == representativeId).ToArray());
        public Task SaveAsync(BrandPricingPolicy policy, CancellationToken ct) { SaveCount++; return Task.CompletedTask; }
        public Task SaveAsync(RepresentativePricingPolicy policy, CancellationToken ct) { SaveCount++; return Task.CompletedTask; }
        public Task SaveAsync(RepresentativeProductDiscount discount, CancellationToken ct) { SaveCount++; return Task.CompletedTask; }
    }

    private sealed class ProductReader(long? brandId) : IProductBrandReader
    {
        public Task<long?> GetBrandIdAsync(long productId, CancellationToken ct) => Task.FromResult(brandId);
    }

    private sealed class RepresentativeReader(RepresentativePricingIdentity? identity)
        : IRepresentativePricingReader
    {
        public Task<RepresentativePricingIdentity?> GetAsync(long representativeId, CancellationToken ct) =>
            Task.FromResult(identity?.Id == representativeId ? identity : null);
    }

    private sealed class CompositionReader(ProductPricingComposition? composition)
        : IProductPricingCompositionReader
    {
        public Task<ProductPricingComposition?> GetAsync(long productId, CancellationToken ct) =>
            Task.FromResult(composition?.ProductId == productId ? composition : null);
    }

    private sealed class EffectivePriceReader(IReadOnlyList<EffectiveItemPrice> prices)
        : IEffectiveItemPriceReader
    {
        public Task<IReadOnlyList<EffectiveItemPrice>> GetAsync(long brandId,
            IReadOnlyCollection<long> localItemIds, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<EffectiveItemPrice>>(prices.Where(x =>
                x.BrandId == brandId && localItemIds.Contains(x.LocalItemId)).ToArray());
    }

    private sealed class OverrideStore : IPriceOverrideStore
    {
        public Task<ProductFactoryPriceOverride?> GetAsync(long brandId, long productId,
            CancellationToken ct) => Task.FromResult<ProductFactoryPriceOverride?>(null);
        public Task SaveAsync(ProductFactoryPriceOverride priceOverride, CancellationToken ct) =>
            throw new InvalidOperationException("Preview must not save an override.");
    }
}
