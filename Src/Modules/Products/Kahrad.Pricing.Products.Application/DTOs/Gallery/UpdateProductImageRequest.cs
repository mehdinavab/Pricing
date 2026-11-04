namespace Kahrad.Pricing.Products.Application.DTOs.Gallery;

public class UpdateProductImageRequest
{
    public string ImageUrl { get; init; } = null!;
    public string? AltText { get; init; }
    public int DisplayOrder { get; init; }
    public string? Description { get; init; }
}
