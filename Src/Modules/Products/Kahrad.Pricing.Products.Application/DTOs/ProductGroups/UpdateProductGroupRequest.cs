namespace Kahrad.Pricing.Products.Application.DTOs.ProductGroups;

public class UpdateProductGroupRequest
{
    public string Name { get; init; } = null!;
    public string Code { get; init; } = null!;
    public decimal PriceEffectPercentage { get; init; }
    public string? Description { get; init; }
}
