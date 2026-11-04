namespace Kahrad.Pricing.Products.Application.DTOs.Products;

public class ProductPriceCalculationDto
{
    public long ProductId { get; init; }
    public string ProductCode { get; init; } = null!;
    public string ProductName { get; init; } = null!;
    public decimal BasePrice { get; init; }

    // Group Effect
    public long? GroupId { get; init; }
    public string? GroupName { get; init; }
    public decimal GroupPriceEffectPercentage { get; init; }
    public decimal GroupPriceEffectAmount { get; init; }

    // Beauty Factor
    public decimal BeautyFactorPercentage { get; init; }
    public decimal BeautyFactorAmount { get; init; }

    // Commercial Factor (Bazargani)
    public decimal CommercialFactorPercentage { get; init; }
    public decimal CommercialFactorAmount { get; init; }

    // Sales Factor
    public decimal SalesFactorPercentage { get; init; }
    public decimal SalesFactorAmount { get; init; }

    // Price-Effect Features
    public decimal FeaturesPercentageTotal { get; init; }
    public decimal FeaturesPercentageAmount { get; init; }
    public decimal FeaturesFixedAmountTotal { get; init; }

    // Estimated Final Price
    public decimal EstimatedTotalPrice { get; init; }
}
