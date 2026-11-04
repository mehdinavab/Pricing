namespace Kahrad.Pricing.Products.Application.DTOs.Products;

public class UpdateProductRequest
{
    public string Name { get; init; } = null!;
    public string Code { get; init; } = null!;
    public long GroupId { get; init; }
    public long? BrandId { get; init; }
    public decimal BasePrice { get; init; }
    public string? Description { get; init; }
}
