namespace Kahrad.Pricing.Pricing.Engine;

/// <summary>Pure, deterministic decimal calculations for confirmed P0 pricing rules.</summary>
public static class PriceMath
{
    public const decimal ConfirmedSellingRoundingStep = 5000m;

    public static decimal RoundUpToStep(decimal amount, decimal step = ConfirmedSellingRoundingStep)
    {
        if (amount < 0m)
            throw new PricingCalculationException(PricingCalculationErrorCode.InvalidPriceAdjustment,
                "A monetary value cannot be negative.");
        if (step <= 0m)
            throw new PricingCalculationException(PricingCalculationErrorCode.InvalidRoundingStep,
                "The rounding step must be positive.");
        try { return checked(decimal.Ceiling(amount / step) * step); }
        catch (OverflowException exception) { throw Overflow(exception); }
    }

    public static decimal ProductionCost(IEnumerable<ItemCostInput> items, long brandId, string currencyCode)
        => CalculateProductionCost(items, brandId, currencyCode).Total;

    private static (decimal Total, IReadOnlyList<PriceComponentResult> Components)
        CalculateProductionCost(IEnumerable<ItemCostInput> items, long brandId, string currencyCode)
    {
        ArgumentNullException.ThrowIfNull(items);
        if (brandId <= 0)
            throw new PricingCalculationException(PricingCalculationErrorCode.BrandMismatch,
                "A valid brand is required.");
        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new PricingCalculationException(PricingCalculationErrorCode.CurrencyMismatch,
                "A currency code is required.");

        var normalizedCurrency = currencyCode.Trim().ToUpperInvariant();
        var seenItemIds = new HashSet<long>();
        var hasItems = false;
        var total = 0m;
        var components = new List<PriceComponentResult>();
        try
        {
            foreach (var item in items)
            {
                hasItems = true;
                if (item.BrandId != brandId)
                    throw Failure(PricingCalculationErrorCode.BrandMismatch,
                        "An item price belongs to another brand.", item.ItemId);
                if (item.ItemId > 0 && !seenItemIds.Add(item.ItemId))
                    throw Failure(PricingCalculationErrorCode.DuplicateItem,
                        "A pricing item appears more than once.", item.ItemId);
                if (item.UnitPrice is null)
                    throw Failure(PricingCalculationErrorCode.ItemPriceMissing,
                        "An effective item price is missing.", item.ItemId);
                if (item.UnitPrice < 0m)
                    throw Failure(PricingCalculationErrorCode.InvalidItemPrice,
                        "An item price cannot be negative.", item.ItemId);
                if (item.QuantityUsed <= 0m)
                    throw Failure(PricingCalculationErrorCode.InvalidQuantity,
                        "An item quantity must be positive.", item.ItemId);
                if (item.UnitConversionFactor is null)
                    throw Failure(PricingCalculationErrorCode.UnitConversionMissing,
                        "An item unit conversion is missing.", item.ItemId);
                if (item.UnitConversionFactor <= 0m)
                    throw Failure(PricingCalculationErrorCode.InvalidUnitConversion,
                        "An item unit conversion must be positive.", item.ItemId);
                if (!string.Equals(item.CurrencyCode?.Trim(), normalizedCurrency,
                        StringComparison.OrdinalIgnoreCase))
                    throw Failure(PricingCalculationErrorCode.CurrencyMismatch,
                        "Item prices must use one currency.", item.ItemId);

                var contribution = checked(checked(item.UnitPrice.Value * item.QuantityUsed) *
                    item.UnitConversionFactor.Value);
                total = checked(total + contribution);
                components.Add(new PriceComponentResult(item.ItemId, item.UnitPrice.Value,
                    item.QuantityUsed, item.UnitConversionFactor.Value, contribution,
                    item.SourcePriceVersion));
            }
        }
        catch (OverflowException exception) { throw Overflow(exception); }

        if (!hasItems)
            throw new PricingCalculationException(PricingCalculationErrorCode.ProductHasNoComponents,
                "Production cost requires at least one priced item.");
        return (total, components.AsReadOnly());
    }

    public static CustomerPriceResult CustomerSellingPrice(decimal representativePurchasePrice,
        decimal representativeSalesPercentage, decimal generalDiscountPercentage,
        decimal productDiscountPercentage, decimal roundingStep = ConfirmedSellingRoundingStep)
    {
        if (representativePurchasePrice < 0m)
            throw new PricingCalculationException(PricingCalculationErrorCode.InvalidRepresentativePolicy,
                "Representative purchase price cannot be negative.");
        ValidatePercentage(representativeSalesPercentage, "Representative sales percentage");
        ValidatePercentage(generalDiscountPercentage, "General discount percentage");
        ValidatePercentage(productDiscountPercentage, "Product discount percentage");
        RequireConfirmedRoundingStep(roundingStep);

        try
        {
            var netPercentage = checked(representativeSalesPercentage -
                generalDiscountPercentage - productDiscountPercentage);
            var salesAmount = checked(representativePurchasePrice * representativeSalesPercentage / 100m);
            var generalDiscount = checked(representativePurchasePrice * generalDiscountPercentage / 100m);
            var productDiscount = checked(representativePurchasePrice * productDiscountPercentage / 100m);
            var raw = checked(representativePurchasePrice + salesAmount - generalDiscount - productDiscount);
            if (netPercentage < 0m || raw < representativePurchasePrice)
                throw new PricingCalculationException(PricingCalculationErrorCode.DiscountBelowPurchasePrice,
                    "Representative discounts would sell below the representative purchase price.");
            return new CustomerPriceResult(netPercentage, salesAmount, generalDiscount,
                productDiscount, raw, RoundUpToStep(raw, roundingStep));
        }
        catch (OverflowException exception) { throw Overflow(exception); }
    }

    public static PricingCalculationResult Calculate(PricingCalculationInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        if (input.BrandId <= 0 || input.ProductId <= 0 || input.RepresentativeId <= 0)
            throw new PricingCalculationException(PricingCalculationErrorCode.BrandMismatch,
                "Valid brand, product and representative identifiers are required.");
        if (string.IsNullOrWhiteSpace(input.CurrencyCode))
            throw new PricingCalculationException(PricingCalculationErrorCode.CurrencyMismatch,
                "A currency code is required.");
        ValidateFactoryPolicy(input.FactoryPolicy);
        ValidateOverride(input.FactoryOverride);
        if (input.WarrantyAmount < 0m)
            throw new PricingCalculationException(PricingCalculationErrorCode.InvalidWarrantyAmount,
                "Warranty amount cannot be negative.");

        var production = CalculateProductionCost(input.Components, input.BrandId, input.CurrencyCode);
        var productionCost = production.Total;
        try
        {
            var factoryPercentTotal = checked(input.FactoryPolicy.BeautyPercentage +
                input.FactoryPolicy.FactorySalesPercentage + input.FactoryPolicy.CommercePercentage);
            var calculatedFactoryBase = checked(productionCost * (1m + factoryPercentTotal / 100m));
            var factoryAfterFactor = checked(calculatedFactoryBase * input.FactoryPolicy.RepresentativeFactor);
            var factoryBase = input.FactoryOverride.Mode switch
            {
                FactoryPriceSelection.Calculated => factoryAfterFactor,
                FactoryPriceSelection.Fixed => input.FactoryOverride.FixedFactorySellingPrice!.Value,
                _ => throw new PricingCalculationException(PricingCalculationErrorCode.InvalidPriceAdjustment,
                    "Factory price mode is invalid.")
            };
            var rawFactoryPrice = checked(factoryBase + input.FactoryOverride.ManualFactoryPriceAdjustment +
                input.WarrantyAmount);
            var representativePrice = RoundUpToStep(rawFactoryPrice,
                input.FactoryPolicy.FactoryRoundingStep);
            var customer = CustomerSellingPrice(representativePrice,
                input.RepresentativePolicy.SalesPercentage,
                input.RepresentativePolicy.GeneralDiscountPercentage,
                input.RepresentativePolicy.ProductDiscountPercentage,
                input.RepresentativePolicy.CustomerRoundingStep);

            return new PricingCalculationResult(input.BrandId, input.ProductId, input.RepresentativeId,
                input.CurrencyCode.Trim().ToUpperInvariant(), production.Components,
                productionCost, factoryPercentTotal,
                calculatedFactoryBase, input.FactoryPolicy.RepresentativeFactor, factoryAfterFactor,
                input.FactoryOverride.Mode, input.FactoryOverride.FixedFactorySellingPrice,
                input.FactoryOverride.ManualFactoryPriceAdjustment, input.WarrantyAmount,
                rawFactoryPrice, representativePrice, customer.SalesAmount,
                customer.GeneralDiscountAmount, customer.ProductDiscountAmount,
                customer.NetPercentage, customer.RawPrice, customer.RoundedPrice,
                input.FactoryPolicy.FactoryRoundingStep,
                input.RepresentativePolicy.CustomerRoundingStep);
        }
        catch (OverflowException exception) { throw Overflow(exception); }
    }

    private static void ValidateFactoryPolicy(BrandFactoryPolicyInput policy)
    {
        ArgumentNullException.ThrowIfNull(policy);
        if (policy.BeautyPercentage < 0m || policy.FactorySalesPercentage < 0m ||
            policy.CommercePercentage < 0m || policy.RepresentativeFactor <= 0m)
            throw new PricingCalculationException(PricingCalculationErrorCode.InvalidFactoryPolicy,
                "Factory percentages must be nonnegative and representative factor must be positive.");
        RequireConfirmedRoundingStep(policy.FactoryRoundingStep);
    }

    private static void ValidateOverride(FactoryOverrideInput priceOverride)
    {
        ArgumentNullException.ThrowIfNull(priceOverride);
        if (priceOverride.ManualFactoryPriceAdjustment < 0m)
            throw new PricingCalculationException(PricingCalculationErrorCode.InvalidPriceAdjustment,
                "Manual factory price adjustment cannot be negative.");
        if (priceOverride.Mode == FactoryPriceSelection.Fixed &&
            priceOverride.FixedFactorySellingPrice is not > 0m)
            throw new PricingCalculationException(PricingCalculationErrorCode.FixedPriceRequired,
                "A positive fixed factory selling price is required in fixed mode.");
        if (priceOverride.Mode == FactoryPriceSelection.Calculated &&
            priceOverride.FixedFactorySellingPrice is not null)
            throw new PricingCalculationException(PricingCalculationErrorCode.InvalidPriceAdjustment,
                "Fixed factory selling price is not valid in calculated mode.");
        if (!Enum.IsDefined(priceOverride.Mode))
            throw new PricingCalculationException(PricingCalculationErrorCode.InvalidPriceAdjustment,
                "Factory price mode is invalid.");
    }

    private static void ValidatePercentage(decimal percentage, string name)
    {
        if (percentage is < 0m or > 100m)
            throw new PricingCalculationException(PricingCalculationErrorCode.InvalidRepresentativePolicy,
                $"{name} must be between 0 and 100.");
    }

    private static void RequireConfirmedRoundingStep(decimal step)
    {
        if (step != ConfirmedSellingRoundingStep)
            throw new PricingCalculationException(PricingCalculationErrorCode.InvalidRoundingStep,
                $"Selling-price rounding step must be {ConfirmedSellingRoundingStep}.");
    }

    private static PricingCalculationException Failure(PricingCalculationErrorCode errorCode,
        string message, long itemId) => new(errorCode, message, itemId > 0 ? itemId : null);

    private static PricingCalculationException Overflow(OverflowException exception) =>
        new(PricingCalculationErrorCode.DecimalOverflow,
            "Pricing arithmetic exceeded decimal capacity.", exception);
}
