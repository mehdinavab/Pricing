using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.AccountingIntegration.Infrastructure.Persistence;

public sealed class AccountingRepresentativeReader(AccountingDbContext db) : IAccountingRepresentativeReader
{
    public Task<ExternalRepresentativeReference?> GetAsync(long id, CancellationToken ct) =>
        db.Representatives.AsNoTracking().Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new ExternalRepresentativeReference(x.Id, x.BrandId, x.ExternalRepresentativeId,
                x.Name, x.Code, x.IsPresentInLatestSync)).SingleOrDefaultAsync(ct);

    public async Task<IReadOnlyList<ExternalRepresentativeReference>> SearchAsync(long brandId, string? search,
        int page, int pageSize, CancellationToken ct)
    {
        var query = db.Representatives.AsNoTracking().Where(x => x.BrandId == brandId && !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.ExternalRepresentativeId.Contains(search) || x.Name.Contains(search) ||
                (x.Code != null && x.Code.Contains(search)));
        return await query.OrderBy(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new ExternalRepresentativeReference(x.Id, x.BrandId, x.ExternalRepresentativeId,
                x.Name, x.Code, x.IsPresentInLatestSync)).ToListAsync(ct);
    }
}
