namespace Kahrad.Pricing.Products.Application.DTOs.ProductGroups;

public class CreateProductGroupRequest
{
    public string Name { get; init; } = null!;
    public string Code { get; init; } = null!;
    public decimal PriceEffectPercentage { get; init; } = 0;
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
}
