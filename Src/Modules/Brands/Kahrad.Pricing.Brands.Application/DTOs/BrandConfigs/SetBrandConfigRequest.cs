using Kahrad.Pricing.Brands.Domain.Enums;

namespace Kahrad.Pricing.Brands.Application.DTOs.BrandConfigs;

public sealed class SetBrandConfigRequest
{
    public string Key { get; init; } = null!;
    public string DisplayName { get; init; } = null!;
    public string Value { get; init; } = null!;
    public BrandConfigDataType DataType { get; init; } = BrandConfigDataType.String;
    public string? Category { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
}
