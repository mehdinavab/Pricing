namespace Kahrad.Pricing.Pricing.Engine;

public enum FactoryPriceSelection { Calculated, Fixed }

public sealed record ItemCostInput(long BrandId, decimal? UnitPrice, decimal QuantityUsed,
    string CurrencyCode, decimal? UnitConversionFactor = 1m, long ItemId = 0,
    long? SourcePriceVersion = null);

public sealed record PriceComponentResult(long ItemId, decimal UnitPrice,
    decimal QuantityUsed, decimal UnitConversionFactor, decimal Contribution,
    long? SourcePriceVersion);

public sealed record BrandFactoryPolicyInput(decimal BeautyPercentage, decimal FactorySalesPercentage,
    decimal CommercePercentage, decimal RepresentativeFactor = 1m,
    decimal FactoryRoundingStep = PriceMath.ConfirmedSellingRoundingStep);

public sealed record FactoryOverrideInput(FactoryPriceSelection Mode,
    decimal? FixedFactorySellingPrice = null, decimal ManualFactoryPriceAdjustment = 0m);

public sealed record RepresentativePolicyInput(decimal SalesPercentage,
    decimal GeneralDiscountPercentage, decimal ProductDiscountPercentage = 0m,
    decimal CustomerRoundingStep = PriceMath.ConfirmedSellingRoundingStep);

public sealed record PricingCalculationInput(long BrandId, long ProductId, long RepresentativeId,
    string CurrencyCode, IReadOnlyList<ItemCostInput> Components,
    BrandFactoryPolicyInput FactoryPolicy, FactoryOverrideInput FactoryOverride,
    RepresentativePolicyInput RepresentativePolicy, decimal WarrantyAmount = 0m);

public sealed record PricingCalculationResult(
    long BrandId, long ProductId, long RepresentativeId, string CurrencyCode,
    IReadOnlyList<PriceComponentResult> Components, decimal ProductionCost,
    decimal FactoryPercentTotal, decimal CalculatedFactoryBase,
    decimal RepresentativeFactor, decimal FactoryAfterFactor,
    FactoryPriceSelection FactoryPriceMode, decimal? FixedFactorySellingPrice,
    decimal ManualFactoryPriceAdjustment, decimal WarrantyAmount, decimal FactoryRawSellingPrice,
    decimal RepresentativePrice, decimal RepresentativeSalesAmount, decimal GeneralDiscountAmount,
    decimal ProductDiscountAmount, decimal NetRepresentativePercentage, decimal RawCustomerPrice,
    decimal CustomerSellingPrice, decimal FactoryRoundingStep, decimal CustomerRoundingStep);

public sealed record CustomerPriceResult(decimal NetPercentage, decimal SalesAmount,
    decimal GeneralDiscountAmount, decimal ProductDiscountAmount, decimal RawPrice,
    decimal RoundedPrice);
