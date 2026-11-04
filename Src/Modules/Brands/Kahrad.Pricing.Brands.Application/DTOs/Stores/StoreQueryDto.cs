using Sieve.Models;

namespace Kahrad.Pricing.Brands.Application.DTOs.Stores;

public sealed class StoreQueryDto : SieveModel
{
    public long? BrandId { get; init; }
}
