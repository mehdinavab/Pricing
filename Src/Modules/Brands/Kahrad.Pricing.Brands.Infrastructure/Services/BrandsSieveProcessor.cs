using Kahrad.Pricing.Brands.Domain.Entities;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;

namespace Kahrad.Pricing.Brands.Infrastructure.Services;

public sealed class BrandsSieveProcessor(IOptions<SieveOptions> options)
    : SieveProcessor(options)
{
    protected override SievePropertyMapper MapProperties(
        SievePropertyMapper mapper)
    {
        mapper = base.MapProperties(mapper);

        mapper.Property<Brand>(b => b.Id).CanSort();
        mapper.Property<Brand>(b => b.Name).CanFilter().CanSort();
        mapper.Property<Brand>(b => b.Code).CanFilter().CanSort();
        mapper.Property<Brand>(b => b.IsActive).CanFilter().CanSort();
        mapper.Property<Brand>(b => b.ActivePriceVersion).CanFilter().CanSort();
        mapper.Property<Brand>(b => b.CreatedAtUtc).CanSort();

        mapper.Property<Store>(s => s.Id).CanSort();
        mapper.Property<Store>(s => s.BrandId).CanFilter().CanSort();
        mapper.Property<Store>(s => s.Name).CanFilter().CanSort();
        mapper.Property<Store>(s => s.Code).CanFilter().CanSort();
        mapper.Property<Store>(s => s.OwnerName).CanFilter().CanSort();
        mapper.Property<Store>(s => s.City).CanFilter().CanSort();
        mapper.Property<Store>(s => s.Province).CanFilter().CanSort();
        mapper.Property<Store>(s => s.IsActive).CanFilter().CanSort();
        mapper.Property<Store>(s => s.PriceAdjustmentType).CanFilter().CanSort();
        mapper.Property<Store>(s => s.PriceAdjustmentPercentage).CanFilter().CanSort();
        mapper.Property<Store>(s => s.CreatedAtUtc).CanSort();

        mapper.Property<BrandConfig>(c => c.Id).CanSort();
        mapper.Property<BrandConfig>(c => c.BrandId).CanFilter().CanSort();
        mapper.Property<BrandConfig>(c => c.Key).CanFilter().CanSort();
        mapper.Property<BrandConfig>(c => c.DisplayName).CanFilter().CanSort();
        mapper.Property<BrandConfig>(c => c.Category).CanFilter().CanSort();
        mapper.Property<BrandConfig>(c => c.IsActive).CanFilter().CanSort();

        return mapper;
    }
}
