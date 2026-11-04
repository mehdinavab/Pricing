using Sieve.Models;

namespace Kahrad.Pricing.Products.Application.DTOs.Products;

public class ProductQueryDto : SieveModel
{
    public long? GroupId { get; init; }
    public long? BrandId { get; init; }
    public bool? IsActive { get; init; }
}
