namespace Kahrad.Pricing.Products.Application.DTOs.Gallery;

public class AddProductImageRequest
{
    public string ImageUrl { get; init; } = null!;
    public string? AltText { get; init; }
    public int DisplayOrder { get; init; } = 0;
    public bool IsPrimary { get; init; } = false;
    public string? Description { get; init; }
}
