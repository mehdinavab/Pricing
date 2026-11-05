using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Kahrad.Pricing.AccountingIntegration.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.AccountingIntegration.Infrastructure.Persistence;

public sealed class AccountingConnectionStore(AccountingDbContext db) : IAccountingConnectionStore
{
    public async Task<IReadOnlyList<AccountingConnectionView>> ListAsync(CancellationToken ct) =>
        await db.Connections.AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.BrandId)
            .Select(x => new AccountingConnectionView(x.BrandId, x.ProviderCode, x.BaseUrl,
                x.ApiKeySecretReference != null, x.IsEnabled)).ToListAsync(ct);

    public Task<AccountingBrandConnection?> GetByBrandAsync(long brandId, CancellationToken ct) =>
        db.Connections.SingleOrDefaultAsync(x => x.BrandId == brandId && !x.IsDeleted, ct);

    public async Task SaveAsync(AccountingBrandConnection connection, CancellationToken ct)
    {
        if (connection.Id == 0) db.Connections.Add(connection);
        await db.SaveChangesAsync(ct);
    }
}
