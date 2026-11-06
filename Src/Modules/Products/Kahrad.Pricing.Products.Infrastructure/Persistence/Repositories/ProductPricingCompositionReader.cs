using Kahrad.Pricing.Products.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Products.Infrastructure.Persistence.Repositories;

public sealed class ProductPricingCompositionReader(ProductsDbContext db)
    : IProductPricingCompositionReader
{
    public async Task<ProductPricingComposition?> GetAsync(long productId, CancellationToken ct)
    {
        var product = await db.Products.AsNoTracking()
            .Where(x => x.Id == productId)
            .Select(x => new { x.Id, x.BrandId })
            .SingleOrDefaultAsync(ct);
        if (product is null) return null;

        var components = await (
            from productItem in db.ProductItems.AsNoTracking()
            join item in db.Items.AsNoTracking() on productItem.ItemId equals item.Id
            where productItem.ProductId == productId
            orderby productItem.ItemId
            select new ProductPricingComponent(item.Id, productItem.QuantityUsed, item.UnitCode))
            .ToListAsync(ct);
        return new ProductPricingComposition(product.Id, product.BrandId, components);
    }
}
