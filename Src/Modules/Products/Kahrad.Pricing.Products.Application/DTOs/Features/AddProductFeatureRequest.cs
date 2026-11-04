namespace Kahrad.Pricing.Products.Application.DTOs.Features;

public class AddProductFeatureRequest
{
    public string Name { get; init; } = null!;
    public string Value { get; init; } = null!;
    public bool HasPriceEffect { get; init; }
    public decimal PriceEffectPercentage { get; init; } = 0;
    public decimal PriceEffectAmount { get; init; } = 0;
    public int DisplayOrder { get; init; } = 0;
    public string? Description { get; init; }
}
