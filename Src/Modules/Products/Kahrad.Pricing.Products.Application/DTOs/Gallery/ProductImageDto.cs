namespace Kahrad.Pricing.Products.Application.DTOs.Gallery;

public class ProductImageDto
{
    public long Id { get; init; }
    public Guid PublicId { get; init; }
    public long ProductId { get; init; }
    public string ImageUrl { get; init; } = null!;
    public string? AltText { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsPrimary { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
