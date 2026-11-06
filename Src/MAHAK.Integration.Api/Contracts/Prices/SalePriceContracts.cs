namespace MAHAK.Integration.Api.Contracts.Prices;

public sealed record SetSalePriceRequest
{
    public string GoodCode { get; init; } = string.Empty;
    public decimal ConsumerPrice { get; init; }
    public decimal ConsumerPriceDollar { get; init; }
    public Guid OperationId { get; init; }
}

public sealed record SetSalePriceResponse(
    string GoodCode,
    bool Updated,
    int AffectedRows);
