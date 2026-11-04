namespace Kahrad.Pricing.Brands.Application.DTOs.Brands;

public sealed class UpdateBrandRequest
{
    public string Name { get; init; } = null!;
    public string Code { get; init; } = null!;
    public string? Description { get; init; }
    public string? LogoUrl { get; init; }
    public string? Website { get; init; }
    public string? SupportPhone { get; init; }
    public string? SupportEmail { get; init; }
}
