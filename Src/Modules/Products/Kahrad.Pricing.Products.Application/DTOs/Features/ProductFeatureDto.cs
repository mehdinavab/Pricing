namespace Kahrad.Pricing.Products.Application.DTOs.Features;

public class ProductFeatureDto
{
    public long Id { get; init; }
    public Guid PublicId { get; init; }
    public long ProductId { get; init; }
    public string Name { get; init; } = null!;
    public string Value { get; init; } = null!;
    public bool HasPriceEffect { get; init; }
    public decimal PriceEffectPercentage { get; init; }
    public decimal PriceEffectAmount { get; init; }
    public int DisplayOrder { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
