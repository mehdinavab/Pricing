namespace Kahrad.Pricing.Products.Application.DTOs.Products;

public class UpdateProductPricingFactorsRequest
{
    public decimal BeautyFactorPercentage { get; init; }
    public decimal CommercialFactorPercentage { get; init; }
    public decimal SalesFactorPercentage { get; init; }
}
