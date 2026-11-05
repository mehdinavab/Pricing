using Kahrad.Pricing.Sales.Application.Contracts;
using Kahrad.Pricing.Sales.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Sales.Infrastructure.Persistence;

public sealed class SaleFinalizationStore(SalesDbContext db) : ISaleFinalizationStore
{
    public Task<SoldProductUnit?> GetByOperationIdAsync(Guid operationId, CancellationToken ct) =>
        db.SoldProductUnits.AsNoTracking().SingleOrDefaultAsync(x => x.OperationId == operationId, ct);

    public async Task<SoldProductUnit> SaveAtomicallyAsync(SoldProductUnit sale, AccountingSalePostOutbox outbox, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        try
        {
            db.SoldProductUnits.Add(sale);
            db.AccountingSalePosts.Add(outbox);
            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return sale;
        }
        catch (DbUpdateException e) when (e.InnerException is SqlException { Number: 2601 or 2627 })
        {
            await transaction.RollbackAsync(ct);
            return await db.SoldProductUnits.AsNoTracking().SingleAsync(x => x.OperationId == sale.OperationId, ct);
        }
    }
}
