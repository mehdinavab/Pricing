namespace MAHAK.Integration.Api.Data.Queries;

public static class SalePriceSql
{
    public const string Update = """
        UPDATE dbo.GoodCostDiscount
        SET
            CostLevel_Sell_1 = @ConsumerPrice,
            CostLevel_Sell_3 = @ConsumerPriceDollar
        WHERE GoodCode = @GoodCode;
        """;
}
