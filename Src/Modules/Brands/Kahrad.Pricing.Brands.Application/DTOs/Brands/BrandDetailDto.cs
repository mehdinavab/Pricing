using Kahrad.Pricing.Brands.Application.DTOs.BrandConfigs;

namespace Kahrad.Pricing.Brands.Application.DTOs.Brands;

public sealed class BrandDetailDto : BrandDto
{
    public IReadOnlyCollection<BrandConfigDto> Configs { get; init; } = [];
    public int StoreCount { get; init; }
}
