namespace Kahrad.Pricing.Products.Application.DTOs.Products;

public class ProductDto
{
    public long Id { get; init; }
    public Guid PublicId { get; init; }
    public string Name { get; init; } = null!;
    public string Code { get; init; } = null!;
    public long GroupId { get; init; }
    public string? GroupName { get; init; }
    public long? BrandId { get; init; }
    public decimal BasePrice { get; init; }
    public decimal BeautyFactorPercentage { get; init; }
    public decimal CommercialFactorPercentage { get; init; }
    public decimal SalesFactorPercentage { get; init; }
    public bool IsActive { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
