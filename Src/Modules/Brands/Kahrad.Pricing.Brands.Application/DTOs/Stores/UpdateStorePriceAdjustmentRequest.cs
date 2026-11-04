using Kahrad.Pricing.Brands.Domain.Enums;

namespace Kahrad.Pricing.Brands.Application.DTOs.Stores;

public sealed class UpdateStorePriceAdjustmentRequest
{
    public PriceAdjustmentType Type { get; init; }
    public decimal Percentage { get; init; }
}
