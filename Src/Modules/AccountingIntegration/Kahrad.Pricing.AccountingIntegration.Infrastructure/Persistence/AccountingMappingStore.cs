using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Kahrad.Pricing.AccountingIntegration.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.AccountingIntegration.Infrastructure.Persistence;

public sealed class AccountingMappingStore(AccountingDbContext db) : IAccountingMappingStore
{
    public Task<ExternalItemReference?> GetExternalItemAsync(long accountingItemId, CancellationToken ct) =>
        db.Items.AsNoTracking().Where(x => x.Id == accountingItemId && !x.IsDeleted)
            .Select(x => new ExternalItemReference(x.Id, x.BrandId, x.ExternalItemId, x.UnitCode))
            .SingleOrDefaultAsync(ct);

    public Task<AccountingItemMapping?> GetByLocalItemAsync(long brandId, long localItemId, CancellationToken ct) =>
        db.ItemMappings.SingleOrDefaultAsync(x => x.BrandId == brandId && x.LocalItemId == localItemId && x.IsActive, ct);

    public Task<AccountingItemMapping?> GetByExternalItemAsync(long brandId, long accountingItemId, CancellationToken ct) =>
        db.ItemMappings.SingleOrDefaultAsync(x => x.BrandId == brandId && x.AccountingItemId == accountingItemId && x.IsActive, ct);

    public async Task AddAsync(AccountingItemMapping mapping, CancellationToken ct)
    {
        db.ItemMappings.Add(mapping);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeactivateAsync(AccountingItemMapping mapping, CancellationToken ct)
    {
        mapping.Deactivate();
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<AccountingItemView>> SearchItemsAsync(long brandId, string? search,
        int page, int pageSize, CancellationToken ct)
    {
        var query = db.Items.AsNoTracking().Where(x => x.BrandId == brandId && !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.ExternalItemId.Contains(search) || x.Name.Contains(search) ||
                (x.Code != null && x.Code.Contains(search)));
        var items = await query.OrderBy(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        var ids = items.Select(x => x.Id).ToArray();
        if (ids.Length == 0) return [];
        var prices = await db.ItemPrices.AsNoTracking().Where(x => ids.Contains(x.AccountingItemId))
            .GroupBy(x => x.AccountingItemId).Select(g => g.OrderByDescending(x => x.Id).First())
            .ToDictionaryAsync(x => x.AccountingItemId, ct);
        var mappings = await db.ItemMappings.AsNoTracking()
            .Where(x => x.BrandId == brandId && x.IsActive && ids.Contains(x.AccountingItemId))
            .ToDictionaryAsync(x => x.AccountingItemId, ct);
        return items.Select(item => new AccountingItemView(item.Id, item.ExternalItemId, item.Name,
            item.Code, item.UnitCode, item.IsPresentInLatestSync,
            prices.GetValueOrDefault(item.Id)?.Amount, prices.GetValueOrDefault(item.Id)?.CurrencyCode,
            mappings.GetValueOrDefault(item.Id)?.LocalItemId)).ToArray();
    }
}
