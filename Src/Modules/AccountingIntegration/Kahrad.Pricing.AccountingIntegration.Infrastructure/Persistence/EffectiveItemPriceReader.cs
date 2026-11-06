using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.AccountingIntegration.Infrastructure.Persistence;

public sealed class EffectiveItemPriceReader(AccountingDbContext db) : IEffectiveItemPriceReader
{
    public async Task<IReadOnlyList<EffectiveItemPrice>> GetAsync(long brandId,
        IReadOnlyCollection<long> localItemIds, CancellationToken ct)
    {
        if (brandId <= 0) throw new ArgumentOutOfRangeException(nameof(brandId));
        if (localItemIds.Count == 0) return [];
        var ids = localItemIds.Distinct().ToArray();

        return await db.ItemMappings.AsNoTracking()
            .Where(mapping => mapping.BrandId == brandId && mapping.IsActive &&
                ids.Contains(mapping.LocalItemId))
            .Select(mapping => new
            {
                mapping.LocalItemId,
                mapping.BrandId,
                mapping.UnitConversionFactor,
                Price = db.ItemPrices.AsNoTracking()
                    .Where(price => price.BrandId == brandId &&
                        price.AccountingItemId == mapping.AccountingItemId)
                    .OrderByDescending(price => price.Id)
                    .Select(price => new
                    {
                        price.Amount,
                        price.CurrencyCode,
                        price.SourceSyncRunId
                    })
                    .FirstOrDefault()
            })
            .Where(x => x.Price != null)
            .Select(x => new EffectiveItemPrice(x.LocalItemId, x.BrandId,
                x.Price!.Amount, x.Price.CurrencyCode, x.UnitConversionFactor,
                x.Price.SourceSyncRunId))
            .ToListAsync(ct);
    }
}
