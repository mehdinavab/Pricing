using Kahrad.Pricing.Products.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Products.Infrastructure.Persistence.Repositories;

public sealed class ProductBrandReader(ProductsDbContext db) : IProductBrandReader
{
    public Task<long?> GetBrandIdAsync(long productId, CancellationToken ct) =>
        db.Products.AsNoTracking().Where(x => x.Id == productId)
            .Select(x => x.BrandId).SingleOrDefaultAsync(ct);
}
