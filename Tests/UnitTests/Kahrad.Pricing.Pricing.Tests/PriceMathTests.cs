using Kahrad.Pricing.Pricing.Engine;
using Xunit;

namespace Kahrad.Pricing.Pricing.Tests;

public sealed class PriceMathTests
{
    [Theory]
    [InlineData(120001, 125000)]
    [InlineData(125000, 125000)]
    [InlineData(125001, 130000)]
    public void Round_up_uses_confirmed_5000_step(decimal input, decimal expected) =>
        Assert.Equal(expected, PriceMath.RoundUpToStep(input));

    [Fact]
    public void Production_cost_uses_only_complete_matching_brand_inputs()
    {
        var cost = PriceMath.ProductionCost([
            new ItemCostInput(1, 120.5m, 2.25m, "IRR"),
            new ItemCostInput(1, 10m, 3m, "IRR")], 1, "IRR");
        Assert.Equal(301.125m, cost);
        Assert.Throws<InvalidOperationException>(() => PriceMath.ProductionCost([
            new ItemCostInput(1, null, 2m, "IRR")], 1, "IRR"));
        Assert.Throws<InvalidOperationException>(() => PriceMath.ProductionCost([
            new ItemCostInput(2, 100m, 1m, "IRR")], 1, "IRR"));
        Assert.Throws<InvalidOperationException>(() => PriceMath.ProductionCost([
            new ItemCostInput(1, 100m, 1m, "USD")], 1, "IRR"));
    }

    [Fact]
    public void Customer_price_uses_algebraic_net_percent_and_pre_rounding_floor()
    {
        var result = PriceMath.CustomerSellingPrice(125000m, 10m, 2m, 1m);
        Assert.Equal(7m, result.NetPercentage);
        Assert.Equal(133750m, result.RawPrice);
        Assert.Equal(135000m, result.RoundedPrice);
        Assert.Throws<InvalidOperationException>(() =>
            PriceMath.CustomerSellingPrice(120001m, 0m, 0.1m, 0m));
    }
}
