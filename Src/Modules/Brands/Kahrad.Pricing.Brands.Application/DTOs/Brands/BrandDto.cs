namespace Kahrad.Pricing.Brands.Application.DTOs.Brands;

public class BrandDto
{
    public long Id { get; init; }
    public Guid PublicId { get; init; }
    public string Name { get; init; } = null!;
    public string Code { get; init; } = null!;
    public string? Description { get; init; }
    public string? LogoUrl { get; init; }
    public bool IsActive { get; init; }
    public int ActivePriceVersion { get; init; }
    public string? Website { get; init; }
    public string? SupportPhone { get; init; }
    public string? SupportEmail { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
