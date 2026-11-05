namespace Kahrad.Pricing.Sales.Domain.Entities;

/// <summary>Finalized monetary values have no mutation path; later prices belong to new sales.</summary>
public sealed class SoldProductUnit
{
    private SoldProductUnit() { }
    public SoldProductUnit(Guid operationId, long brandId, long productId, long representativeId,
        decimal productionCost, decimal representativePrice, decimal customerSellingPrice,
        string currencyCode, long publishedPriceVersion, DateTime finalizedAtUtc)
    {
        if (operationId == Guid.Empty || brandId <= 0 || productId <= 0 || representativeId <= 0 || publishedPriceVersion <= 0)
            throw new ArgumentException("Sale identity and published price version are required.");
        if (productionCost < 0 || representativePrice <= 0 || customerSellingPrice < representativePrice)
            throw new ArgumentException("Finalized sale prices are invalid.");
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);
        OperationId = operationId; BrandId = brandId; ProductId = productId; RepresentativeId = representativeId;
        ProductionCostSnapshot = productionCost; RepresentativePriceSnapshot = representativePrice;
        CustomerSellingPriceSnapshot = customerSellingPrice; CurrencyCode = currencyCode.Trim().ToUpperInvariant();
        PublishedPriceVersion = publishedPriceVersion; FinalizedAtUtc = finalizedAtUtc;
    }

    public long Id { get; private set; }
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public byte[] RowVersion { get; private set; } = [];
    public Guid OperationId { get; private set; }
    public long BrandId { get; private set; }
    public long ProductId { get; private set; }
    public long RepresentativeId { get; private set; }
    public decimal ProductionCostSnapshot { get; private set; }
    public decimal RepresentativePriceSnapshot { get; private set; }
    public decimal CustomerSellingPriceSnapshot { get; private set; }
    public string CurrencyCode { get; private set; } = null!;
    public long PublishedPriceVersion { get; private set; }
    public DateTime FinalizedAtUtc { get; private set; }
}
