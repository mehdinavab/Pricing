using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Kahrad.Pricing.AccountingIntegration.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.AccountingIntegration.Infrastructure.Persistence;

public sealed class AccountingSyncRunReader(AccountingDbContext db) : IAccountingSyncRunReader
{
    public async Task<IReadOnlyList<AccountingSyncRunView>> SearchAsync(long brandId, AccountingSyncType? type,
        int page, int pageSize, CancellationToken ct)
    {
        var query = db.SyncRuns.AsNoTracking().Where(x => x.BrandId == brandId && !x.IsDeleted);
        if (type is not null) query = query.Where(x => x.Type == type);
        return await query.OrderByDescending(x => x.StartedAtUtc).ThenByDescending(x => x.Id)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new AccountingSyncRunView(x.Id, x.BrandId, x.Type, x.Status,
                x.StartedAtUtc, x.CompletedAtUtc, x.RowsFetched, x.RowsChanged,
                x.FullScanCompleted, x.ErrorCategory)).ToListAsync(ct);
    }
}
