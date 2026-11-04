namespace Kahrad.Pricing.Products.Application.DTOs.ProductGroups;

public class ProductGroupDto
{
    public long Id { get; init; }
    public Guid PublicId { get; init; }
    public string Name { get; init; } = null!;
    public string Code { get; init; } = null!;
    public decimal PriceEffectPercentage { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
