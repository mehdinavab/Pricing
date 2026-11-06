using Kahrad.Pricing.Representatives.Application.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Representatives.Infrastructure.Persistence;

public sealed class RepresentativePricingReader(RepresentativesDbContext db)
    : IRepresentativePricingReader
{
    public Task<RepresentativePricingIdentity?> GetAsync(long representativeId,
        CancellationToken ct) => db.Representatives.AsNoTracking()
        .Where(x => x.Id == representativeId)
        .Select(x => new RepresentativePricingIdentity(x.Id, x.BrandId, x.IsActive))
        .SingleOrDefaultAsync(ct);
}
