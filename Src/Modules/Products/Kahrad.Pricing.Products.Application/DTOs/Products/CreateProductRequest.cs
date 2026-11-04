namespace Kahrad.Pricing.Products.Application.DTOs.Products;

public class CreateProductRequest
{
    public string Name { get; init; } = null!;
    public string Code { get; init; } = null!;
    public long GroupId { get; init; }
    public long? BrandId { get; init; }
    public decimal BasePrice { get; init; } = 0;
    public decimal BeautyFactorPercentage { get; init; } = 0;
    public decimal CommercialFactorPercentage { get; init; } = 0;
    public decimal SalesFactorPercentage { get; init; } = 0;
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
}
