using Kahrad.Pricing.Products.Domain.Entities;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;

namespace Kahrad.Pricing.Products.Infrastructure.Services;

public sealed class ProductsSieveProcessor(IOptions<SieveOptions> options)
    : SieveProcessor(options)
{
    protected override SievePropertyMapper MapProperties(
        SievePropertyMapper mapper)
    {
        mapper = base.MapProperties(mapper);

        // ProductGroup
        mapper.Property<ProductGroup>(g => g.Id).CanSort();
        mapper.Property<ProductGroup>(g => g.Name).CanFilter().CanSort();
        mapper.Property<ProductGroup>(g => g.Code).CanFilter().CanSort();
        mapper.Property<ProductGroup>(g => g.PriceEffectPercentage).CanFilter().CanSort();
        mapper.Property<ProductGroup>(g => g.IsActive).CanFilter().CanSort();
        mapper.Property<ProductGroup>(g => g.CreatedAtUtc).CanSort();

        // Product
        mapper.Property<Product>(p => p.Id).CanSort();
        mapper.Property<Product>(p => p.Name).CanFilter().CanSort();
        mapper.Property<Product>(p => p.Code).CanFilter().CanSort();
        mapper.Property<Product>(p => p.GroupId).CanFilter().CanSort();
        mapper.Property<Product>(p => p.BrandId).CanFilter().CanSort();
        mapper.Property<Product>(p => p.BasePrice).CanFilter().CanSort();
        mapper.Property<Product>(p => p.BeautyFactorPercentage).CanFilter().CanSort();
        mapper.Property<Product>(p => p.CommercialFactorPercentage).CanFilter().CanSort();
        mapper.Property<Product>(p => p.SalesFactorPercentage).CanFilter().CanSort();
        mapper.Property<Product>(p => p.IsActive).CanFilter().CanSort();
        mapper.Property<Product>(p => p.CreatedAtUtc).CanSort();

        // ProductFeature
        mapper.Property<ProductFeature>(f => f.Id).CanSort();
        mapper.Property<ProductFeature>(f => f.ProductId).CanFilter().CanSort();
        mapper.Property<ProductFeature>(f => f.Name).CanFilter().CanSort();
        mapper.Property<ProductFeature>(f => f.Value).CanFilter().CanSort();
        mapper.Property<ProductFeature>(f => f.HasPriceEffect).CanFilter().CanSort();
        mapper.Property<ProductFeature>(f => f.PriceEffectPercentage).CanFilter().CanSort();
        mapper.Property<ProductFeature>(f => f.PriceEffectAmount).CanFilter().CanSort();
        mapper.Property<ProductFeature>(f => f.DisplayOrder).CanSort();
        mapper.Property<ProductFeature>(f => f.CreatedAtUtc).CanSort();

        // ProductImage
        mapper.Property<ProductImage>(i => i.Id).CanSort();
        mapper.Property<ProductImage>(i => i.ProductId).CanFilter().CanSort();
        mapper.Property<ProductImage>(i => i.DisplayOrder).CanSort();
        mapper.Property<ProductImage>(i => i.IsPrimary).CanFilter().CanSort();
        mapper.Property<ProductImage>(i => i.CreatedAtUtc).CanSort();

        return mapper;
    }
}
