using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Kahrad.Pricing.Pricing.Application.Contracts;
using Kahrad.Pricing.Pricing.Domain.Entities;
using Kahrad.Pricing.Pricing.Engine;
using Kahrad.Pricing.Products.Application.Abstractions.Persistence;
using Kahrad.Pricing.Representatives.Application.Contracts;

namespace Kahrad.Pricing.Pricing.Application.Services;

public sealed class ProductPricePreviewService(IProductPricingCompositionReader products,
    IEffectiveItemPriceReader itemPrices, IRepresentativePricingReader representatives,
    IPricingPolicyStore policies, IPriceOverrideStore overrides)
{
    public async Task<PricingCalculationResult> CalculateAsync(long productId,
        long representativeId, decimal warrantyAmount, CancellationToken ct)
    {
        var product = await products.GetAsync(productId, ct)
            ?? throw new PricingOperationException("Pricing product was not found.");
        if (product.BrandId is not > 0)
            throw new PricingOperationException("Pricing product has no authoritative brand.");
        var brandId = product.BrandId.Value;

        var representative = await representatives.GetAsync(representativeId, ct)
            ?? throw new PricingOperationException("Pricing representative was not found.");
        if (!representative.IsActive || representative.BrandId != brandId)
            throw new PricingOperationException("Representative and product brands do not match.");

        var brandPolicy = await policies.GetBrandPolicyAsync(brandId, ct)
            ?? throw new PricingOperationException("Brand pricing policy is not configured.");
        var representativePolicy = await policies.GetRepresentativePolicyAsync(brandId,
            representativeId, ct) ?? throw new PricingOperationException(
                "Representative pricing policy is not configured.");
        var productDiscount = await policies.GetProductDiscountAsync(brandId,
            representativeId, productId, ct);
        var priceOverride = await overrides.GetAsync(brandId, productId, ct);

        var effectivePrices = await itemPrices.GetAsync(brandId,
            product.Components.Select(x => x.ItemId).ToArray(), ct);
        var priceByItem = effectivePrices.ToDictionary(x => x.LocalItemId);
        var missingItem = product.Components.FirstOrDefault(x => !priceByItem.ContainsKey(x.ItemId));
        if (missingItem is not null)
            throw new PricingCalculationException(PricingCalculationErrorCode.ItemPriceMissing,
                "An effective brand-specific item price or mapping is missing.", missingItem.ItemId);
        var components = product.Components.Select(component =>
        {
            var price = priceByItem[component.ItemId];
            return new ItemCostInput(brandId, price.Amount, component.QuantityUsed,
                price.CurrencyCode, price.UnitConversionFactor,
                component.ItemId, price.SourceSyncRunId);
        }).ToArray();

        var currencies = effectivePrices.Select(x => x.CurrencyCode.Trim().ToUpperInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        if (currencies.Length != 1)
            throw new PricingCalculationException(PricingCalculationErrorCode.CurrencyMismatch,
                "A preview requires one explicit currency across all effective item prices.");

        return PriceMath.Calculate(new PricingCalculationInput(brandId, productId,
            representativeId, currencies[0], components,
            new BrandFactoryPolicyInput(brandPolicy.BeautyPercentage,
                brandPolicy.FactorySalesPercentage, brandPolicy.CommercePercentage,
                brandPolicy.RepresentativeFactor, brandPolicy.FactoryRoundingStep),
            priceOverride is null
                ? new FactoryOverrideInput(FactoryPriceSelection.Calculated)
                : new FactoryOverrideInput(
                    priceOverride.Mode == FactorySellingPriceMode.Fixed
                        ? FactoryPriceSelection.Fixed : FactoryPriceSelection.Calculated,
                    priceOverride.FixedFactorySellingPrice,
                    priceOverride.ManualFactoryPriceAdjustment),
            new RepresentativePolicyInput(representativePolicy.SalesPercentage,
                representativePolicy.GeneralDiscountPercentage,
                productDiscount?.DiscountPercentage ?? 0m,
                representativePolicy.CustomerRoundingStep),
            warrantyAmount));
    }
}
