using Kahrad.Pricing.Pricing.Application.Contracts;
using Kahrad.Pricing.Pricing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Kahrad.Pricing.Pricing.Application;

namespace Kahrad.Pricing.Pricing.Infrastructure.Persistence;

public sealed class PriceOverrideStore(PricingDbContext db) : IPriceOverrideStore
{
    public Task<ProductFactoryPriceOverride?> GetAsync(long brandId, long productId, CancellationToken ct) =>
        db.ProductFactoryPriceOverrides.SingleOrDefaultAsync(x => x.BrandId == brandId && x.ProductId == productId, ct);

    public async Task SaveAsync(ProductFactoryPriceOverride priceOverride, CancellationToken ct)
    {
        if (priceOverride.Id == 0) db.ProductFactoryPriceOverrides.Add(priceOverride);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateConcurrencyException)
        { throw new PricingOperationException("Price override changed; reload before editing.", true); }
        catch (DbUpdateException e) when (e.InnerException is SqlException { Number: 2601 or 2627 })
        { throw new PricingOperationException("Price override changed; reload before editing.", true); }
    }
}
