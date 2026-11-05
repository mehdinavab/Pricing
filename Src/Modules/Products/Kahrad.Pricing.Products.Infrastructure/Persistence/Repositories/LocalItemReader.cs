using Kahrad.Pricing.Products.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Products.Infrastructure.Persistence.Repositories;

public sealed class LocalItemReader(ProductsDbContext db) : ILocalItemReader
{
    public Task<LocalItemReference?> GetAsync(long itemId, CancellationToken cancellationToken) =>
        db.Items.AsNoTracking().Where(x => x.Id == itemId)
            .Select(x => new LocalItemReference(x.Id, x.Code, x.UnitCode))
            .SingleOrDefaultAsync(cancellationToken);
}
