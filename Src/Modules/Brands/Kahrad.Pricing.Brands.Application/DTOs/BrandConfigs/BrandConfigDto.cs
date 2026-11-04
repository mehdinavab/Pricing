using Kahrad.Pricing.Brands.Domain.Enums;

namespace Kahrad.Pricing.Brands.Application.DTOs.BrandConfigs;

public sealed class BrandConfigDto
{
    public long Id { get; init; }
    public Guid PublicId { get; init; }
    public long BrandId { get; init; }
    public string Key { get; init; } = null!;
    public string DisplayName { get; init; } = null!;
    public string Value { get; init; } = null!;
    public BrandConfigDataType DataType { get; init; }
    public string? Category { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}
