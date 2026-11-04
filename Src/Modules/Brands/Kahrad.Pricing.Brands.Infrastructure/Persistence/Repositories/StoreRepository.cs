using Kahrad.Pricing.Brands.Application.Abstractions.Persistence;
using Kahrad.Pricing.Brands.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Brands.Infrastructure.Persistence.Repositories;

public sealed class StoreRepository(BrandsDbContext context) : IStoreRepository
{
    public async Task<Store?> GetByIdAsync(
        long id,
        bool includeBrand = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Stores.AsQueryable();

        if (includeBrand)
            query = query.Include(x => x.Brand);

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Store?> GetByPublicIdAsync(
        Guid publicId,
        bool includeBrand = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Stores.AsQueryable();

        if (includeBrand)
            query = query.Include(x => x.Brand);

        return await query.FirstOrDefaultAsync(x => x.PublicId == publicId, cancellationToken);
    }

    public async Task<Store?> GetByCodeAsync(
        long brandId,
        string code,
        bool includeBrand = false,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        var query = context.Stores.AsQueryable();

        if (includeBrand)
            query = query.Include(x => x.Brand);

        return await query.FirstOrDefaultAsync(
            x => x.BrandId == brandId && x.Code == normalizedCode,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Store>> GetByBrandIdAsync(
        long brandId,
        CancellationToken cancellationToken = default)
    {
        return await context.Stores
            .AsNoTracking()
            .Include(x => x.Brand)
            .Where(x => x.BrandId == brandId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await context.Stores
            .AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        long brandId,
        string code,
        long? excludeStoreId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return await context.Stores
            .AnyAsync(
                x => x.BrandId == brandId &&
                     x.Code == normalizedCode &&
                     (!excludeStoreId.HasValue || x.Id != excludeStoreId.Value),
                cancellationToken);
    }

    public async Task<long> AddAsync(
        Store store,
        CancellationToken cancellationToken = default)
    {
        await context.Stores.AddAsync(store, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return store.Id;
    }

    public async Task UpdateAsync(
        Store store,
        CancellationToken cancellationToken = default)
    {
        context.Stores.Update(store);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Store store,
        CancellationToken cancellationToken = default)
    {
        context.Stores.Update(store);
        await context.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<Store> GetQueryable()
    {
        return context.Stores.AsQueryable();
    }
}
