using Kahrad.Pricing.Representatives.Application.Contracts;
using Kahrad.Pricing.Representatives.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Kahrad.Pricing.Representatives.Application;

namespace Kahrad.Pricing.Representatives.Infrastructure.Persistence;

public sealed class RepresentativeStore(RepresentativesDbContext db) : IRepresentativeStore
{
    public Task<Representative?> GetAsync(long id, CancellationToken ct) =>
        db.Representatives.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task<Representative?> GetByCodeAsync(long brandId, string code, CancellationToken ct) =>
        db.Representatives.AsNoTracking().SingleOrDefaultAsync(x => x.BrandId == brandId && x.Code == code, ct);
    public Task<RepresentativeAccountingMapping?> GetMappingByExternalAsync(long brandId, long externalId, CancellationToken ct) =>
        db.AccountingMappings.AsNoTracking().SingleOrDefaultAsync(x => x.BrandId == brandId && x.AccountingRepresentativeId == externalId && x.IsActive, ct);
    public Task<RepresentativeAccountingMapping?> GetMappingByLocalAsync(long brandId, long representativeId, CancellationToken ct) =>
        db.AccountingMappings.AsNoTracking().SingleOrDefaultAsync(x => x.BrandId == brandId && x.RepresentativeId == representativeId && x.IsActive, ct);

    public async Task<long> ImportAsync(Representative representative, long accountingRepresentativeId, Guid? actorId, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        try
        {
            db.Representatives.Add(representative);
            await db.SaveChangesAsync(ct);
            db.AccountingMappings.Add(new RepresentativeAccountingMapping(representative.BrandId,
                representative.Id, accountingRepresentativeId, actorId));
            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return representative.Id;
        }
        catch (DbUpdateException e) when (e.InnerException is SqlException { Number: 2601 or 2627 })
        {
            await transaction.RollbackAsync(ct);
            var existing = await db.AccountingMappings.AsNoTracking().SingleOrDefaultAsync(x =>
                x.BrandId == representative.BrandId && x.AccountingRepresentativeId == accountingRepresentativeId && x.IsActive, ct);
            if (existing is not null) return existing.RepresentativeId;
            throw new RepresentativeOperationException("Representative code or accounting mapping already exists.");
        }
    }

    public async Task AddMappingAsync(RepresentativeAccountingMapping mapping, CancellationToken ct)
    {
        db.AccountingMappings.Add(mapping);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException e) when (e.InnerException is SqlException { Number: 2601 or 2627 })
        { throw new RepresentativeOperationException("Representative accounting mapping already exists."); }
    }
}
