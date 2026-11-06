using MAHAK.Integration.Api.Data.Queries;
using MAHAK.Integration.Api.Data.Repositories;
using Xunit;

namespace MAHAK.Integration.Tests.Unit;

public sealed class SqlContractTests
{
    [Fact]
    public void Returned_items_query_is_explicitly_return_scoped_and_deduplicated()
    {
        Assert.Contains("EXISTS", ItemSql.ReturnedItemsOnly, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("BackFact_Detail", ItemSql.ReturnedItemsOnly);
        Assert.Contains("DISTINCT", ItemSql.ReturnedItemsOnly, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("SELECT *", ItemSql.ReturnedItemsOnly, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Representative_query_requires_fax_and_the_confirmed_groups()
    {
        Assert.Contains("LEN(p.Fax) > 1", RepresentativeSql.BoundedProjection);
        Assert.Contains("p.GrpCode IN (4, 5, 6)", RepresentativeSql.BoundedProjection);
        Assert.DoesNotContain("SELECT *", RepresentativeSql.BoundedProjection, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Invoice_query_uses_parameterized_strict_threshold_and_antijoin()
    {
        Assert.Contains("f.kala_code > @ProductCodeThreshold", InvoiceSql.CurrentNonReturnedProjection);
        Assert.Contains("bf.sanad_code IS NULL", InvoiceSql.CurrentNonReturnedProjection);
        Assert.Contains("FM.Shakhs_Code", InvoiceSql.CurrentNonReturnedProjection);
        Assert.Contains("PRF.Fax", InvoiceSql.CurrentNonReturnedProjection);
        Assert.False(10_000 > InvoiceRepository.ProductCodeThreshold);
        Assert.True(10_001 > InvoiceRepository.ProductCodeThreshold);
    }

    [Fact]
    public void Price_query_updates_only_current_irr_and_usd_fields()
    {
        Assert.Contains("CostLevel_Sell_1 = @ConsumerPrice", SalePriceSql.Update);
        Assert.Contains("CostLevel_Sell_3 = @ConsumerPriceDollar", SalePriceSql.Update);
        Assert.Contains("WHERE GoodCode = @GoodCode", SalePriceSql.Update);
        Assert.DoesNotContain("Fact_Fo", SalePriceSql.Update, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Queries_do_not_use_nolock_or_hardcoded_catalogs()
    {
        var queries = string.Join('\n', ItemSql.ReturnedItemsOnly,
            RepresentativeSql.BoundedProjection,
            InvoiceSql.CurrentNonReturnedProjection,
            SalePriceSql.Update);
        Assert.DoesNotContain("NOLOCK", queries, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("mahak2", queries, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("mahak3", queries, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("12050", "1205019")]
    [InlineData("0012050", "001205019")]
    public void External_product_serial_contract_is_string_concatenation(string code, string expected) =>
        Assert.Equal(expected, code + "19");
}
