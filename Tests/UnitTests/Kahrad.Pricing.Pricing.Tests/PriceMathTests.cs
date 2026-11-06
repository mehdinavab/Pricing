using Kahrad.Pricing.Pricing.Engine;
using Kahrad.Pricing.Sales.Domain.Entities;

namespace Kahrad.Pricing.Pricing.Tests;

public sealed class PriceMathTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(120000, 120000)]
    [InlineData(120001, 125000)]
    [InlineData(123456, 125000)]
    [InlineData(125000, 125000)]
    [InlineData(125001, 130000)]
    [InlineData(129999, 130000)]
    [InlineData(1234567, 1235000)]
    public void Confirmed_rounding_examples_are_exact(decimal input, decimal expected) =>
        Assert.Equal(expected, PriceMath.RoundUpToStep(input));

    [Fact]
    public void Case_A_baseline_three_prices()
    {
        var result = Calculate(
            [Item(1, 100000m, 2m, 1), Item(1, 150000m, 1m, 2)],
            new(10m, 15m, 5m), new(FactoryPriceSelection.Calculated),
            new(20m, 5m, 3m), 50000m);

        Assert.Equal(350000m, result.ProductionCost);
        Assert.Equal(455000m, result.CalculatedFactoryBase);
        Assert.Equal(505000m, result.FactoryRawSellingPrice);
        Assert.Equal(505000m, result.RepresentativePrice);
        Assert.Equal(12m, result.NetRepresentativePercentage);
        Assert.Equal(565600m, result.RawCustomerPrice);
        Assert.Equal(570000m, result.CustomerSellingPrice);
    }

    [Fact]
    public void Case_B_larger_end_to_end_example()
    {
        var result = Calculate([Item(1, 10000000m, 1m)], new(10m, 15m, 5m),
            new(FactoryPriceSelection.Calculated), new(20m, 5m, 3m), 500000m);

        Assert.Equal(10000000m, result.ProductionCost);
        Assert.Equal(13000000m, result.CalculatedFactoryBase);
        Assert.Equal(13500000m, result.RepresentativePrice);
        Assert.Equal(15120000m, result.RawCustomerPrice);
        Assert.Equal(15120000m, result.CustomerSellingPrice);
    }

    [Fact]
    public void Case_C_fixed_factory_price_and_manual_addition_do_not_change_cost()
    {
        var result = Calculate([Item(1, 800000m, 1m)], new(10m, 0m, 0m),
            new(FactoryPriceSelection.Fixed, 1234000m, 5000m), new(0m, 0m));

        Assert.Equal(800000m, result.ProductionCost);
        Assert.Equal(880000m, result.CalculatedFactoryBase);
        Assert.Equal(1239000m, result.FactoryRawSellingPrice);
        Assert.Equal(1240000m, result.RepresentativePrice);
        Assert.Equal(1240000m, result.CustomerSellingPrice);
    }

    [Fact]
    public void Case_D_below_purchase_fails_before_rounding()
    {
        var exception = Assert.Throws<PricingCalculationException>(() =>
            PriceMath.CustomerSellingPrice(1000000m, 10m, 5m, 6m));
        Assert.Equal(PricingCalculationErrorCode.DiscountBelowPurchasePrice, exception.ErrorCode);
    }

    [Fact]
    public void Case_E_zero_margin_is_allowed()
    {
        var result = PriceMath.CustomerSellingPrice(1000000m, 20m, 5m, 15m);
        Assert.Equal(0m, result.NetPercentage);
        Assert.Equal(1000000m, result.RawPrice);
        Assert.Equal(1000000m, result.RoundedPrice);
    }

    [Fact]
    public void Case_F_customer_rounding_occurs_after_algebraic_calculation()
    {
        var result = PriceMath.CustomerSellingPrice(125000m, 12.5m, 5m, 2.5m);
        Assert.Equal(5m, result.NetPercentage);
        Assert.Equal(131250m, result.RawPrice);
        Assert.Equal(135000m, result.RoundedPrice);
    }

    [Fact]
    public void Case_G_new_fixed_price_does_not_change_cost_or_finalized_snapshot()
    {
        var finalized = new SoldProductUnit(Guid.NewGuid(), 1, 2, 3,
            800000m, 1000000m, 1000000m, "IRR", 1, DateTime.UnixEpoch);
        var first = Calculate([Item(1, 800000m, 1m)], new(0m, 0m, 0m),
            new(FactoryPriceSelection.Fixed, 1000000m), new(0m, 0m));
        var second = Calculate([Item(1, 800000m, 1m)], new(0m, 0m, 0m),
            new(FactoryPriceSelection.Fixed, 1500000m), new(0m, 0m));

        Assert.Equal(first.ProductionCost, second.ProductionCost);
        Assert.NotEqual(first.RepresentativePrice, second.RepresentativePrice);
        Assert.Equal(800000m, finalized.ProductionCostSnapshot);
        Assert.Equal(1000000m, finalized.RepresentativePriceSnapshot);
        Assert.Equal(1000000m, finalized.CustomerSellingPriceSnapshot);
    }

    [Fact]
    public void Case_H_item_prices_are_isolated_by_brand()
    {
        var brandA = PriceMath.ProductionCost([Item(1, 100000m, 2m, 10)], 1, "IRR");
        var brandB = PriceMath.ProductionCost([Item(2, 130000m, 2m, 10)], 2, "IRR");
        Assert.Equal(200000m, brandA);
        Assert.Equal(260000m, brandB);

        var mismatch = Assert.Throws<PricingCalculationException>(() =>
            PriceMath.ProductionCost([Item(1, 100000m, 2m, 10)], 2, "IRR"));
        Assert.Equal(PricingCalculationErrorCode.BrandMismatch, mismatch.ErrorCode);
    }

    [Fact]
    public void Case_I_product_discounts_are_independent_per_representative()
    {
        var repA = PriceMath.CustomerSellingPrice(1000000m, 20m, 5m, 3m);
        var repB = PriceMath.CustomerSellingPrice(1000000m, 20m, 5m, 10m);
        Assert.Equal(1120000m, repA.RawPrice);
        Assert.Equal(1120000m, repA.RoundedPrice);
        Assert.Equal(1050000m, repB.RawPrice);
        Assert.Equal(1050000m, repB.RoundedPrice);
    }

    [Fact]
    public void Case_J_fractional_quantity_preserves_cost_precision()
    {
        var result = Calculate([Item(1, 12345.67m, 0.5m)], new(0m, 0m, 0m),
            new(FactoryPriceSelection.Calculated), new(0m, 0m));
        Assert.Equal(6172.835m, result.ProductionCost);
        Assert.Equal(10000m, result.RepresentativePrice);
        Assert.Equal(10000m, result.CustomerSellingPrice);
    }

    [Fact]
    public void Unit_conversion_is_applied_once_and_invalid_conversion_is_rejected()
    {
        Assert.Equal(300m, PriceMath.ProductionCost(
            [new ItemCostInput(1, 100m, 2m, "IRR", 1.5m, 1)], 1, "IRR"));
        var missing = Assert.Throws<PricingCalculationException>(() => PriceMath.ProductionCost(
            [new ItemCostInput(1, 100m, 2m, "IRR", null, 1)], 1, "IRR"));
        Assert.Equal(PricingCalculationErrorCode.UnitConversionMissing, missing.ErrorCode);
        var invalid = Assert.Throws<PricingCalculationException>(() => PriceMath.ProductionCost(
            [new ItemCostInput(1, 100m, 2m, "IRR", 0m, 1)], 1, "IRR"));
        Assert.Equal(PricingCalculationErrorCode.InvalidUnitConversion, invalid.ErrorCode);
    }

    [Fact]
    public void Missing_duplicate_and_mixed_currency_inputs_fail_with_structured_codes()
    {
        AssertCode(PricingCalculationErrorCode.ItemPriceMissing,
            () => PriceMath.ProductionCost([Item(1, null, 1m)], 1, "IRR"));
        AssertCode(PricingCalculationErrorCode.DuplicateItem,
            () => PriceMath.ProductionCost([Item(1, 1m, 1m), Item(1, 2m, 1m)], 1, "IRR"));
        AssertCode(PricingCalculationErrorCode.CurrencyMismatch,
            () => PriceMath.ProductionCost([new ItemCostInput(1, 1m, 1m, "USD", 1m, 1)], 1, "IRR"));
    }

    [Fact]
    public void Manual_and_warranty_amounts_apply_once_after_factory_formula()
    {
        var result = Calculate([Item(1, 100000m, 1m)], new(10m, 10m, 10m),
            new(FactoryPriceSelection.Calculated, null, 5000m), new(0m, 0m), 10000m);
        Assert.Equal(130000m, result.CalculatedFactoryBase);
        Assert.Equal(145000m, result.FactoryRawSellingPrice);
        Assert.Equal(145000m, result.RepresentativePrice);
        Assert.Equal(100000m, result.ProductionCost);
    }

    [Fact]
    public void Invalid_rounding_fixed_adjustment_warranty_and_percentages_are_rejected()
    {
        AssertCode(PricingCalculationErrorCode.InvalidRoundingStep,
            () => PriceMath.RoundUpToStep(1m, 0m));
        AssertCode(PricingCalculationErrorCode.FixedPriceRequired,
            () => Calculate([Item(1, 1m, 1m)], new(0m, 0m, 0m),
                new(FactoryPriceSelection.Fixed), new(0m, 0m)));
        AssertCode(PricingCalculationErrorCode.InvalidPriceAdjustment,
            () => Calculate([Item(1, 1m, 1m)], new(0m, 0m, 0m),
                new(FactoryPriceSelection.Calculated, null, -1m), new(0m, 0m)));
        AssertCode(PricingCalculationErrorCode.InvalidWarrantyAmount,
            () => Calculate([Item(1, 1m, 1m)], new(0m, 0m, 0m),
                new(FactoryPriceSelection.Calculated), new(0m, 0m), -1m));
        AssertCode(PricingCalculationErrorCode.InvalidRepresentativePolicy,
            () => PriceMath.CustomerSellingPrice(100m, 101m, 0m, 0m));
    }

    [Fact]
    public void Same_input_is_deterministic_and_has_no_clock_dependency()
    {
        var input = Input([Item(1, 100m, 2m)], new(10m, 5m, 5m),
            new(FactoryPriceSelection.Calculated), new(20m, 5m, 3m));
        var first = PriceMath.Calculate(input);
        var second = PriceMath.Calculate(input);
        Assert.Equal(first with { Components = second.Components }, second);
        Assert.Equal(first.Components.Count, second.Components.Count);
        for (var index = 0; index < first.Components.Count; index++)
            Assert.Equal(first.Components[index], second.Components[index]);
    }

    [Fact]
    public void Decimal_overflow_has_a_machine_readable_error()
    {
        AssertCode(PricingCalculationErrorCode.DecimalOverflow,
            () => PriceMath.ProductionCost([Item(1, decimal.MaxValue, 2m)], 1, "IRR"));
    }

    private static PricingCalculationResult Calculate(IReadOnlyList<ItemCostInput> items,
        BrandFactoryPolicyInput factory, FactoryOverrideInput priceOverride,
        RepresentativePolicyInput representative, decimal warranty = 0m) =>
        PriceMath.Calculate(Input(items, factory, priceOverride, representative, warranty));

    private static PricingCalculationInput Input(IReadOnlyList<ItemCostInput> items,
        BrandFactoryPolicyInput factory, FactoryOverrideInput priceOverride,
        RepresentativePolicyInput representative, decimal warranty = 0m) =>
        new(1, 2, 3, "IRR", items, factory, priceOverride, representative, warranty);

    private static ItemCostInput Item(long brandId, decimal? price, decimal quantity,
        long itemId = 1) => new(brandId, price, quantity, "IRR", 1m, itemId);

    private static void AssertCode(PricingCalculationErrorCode expected, Action action) =>
        Assert.Equal(expected, Assert.Throws<PricingCalculationException>(action).ErrorCode);
}
