using Kahrad.Pricing.Brands.Application.Abstractions.Persistence;
using Kahrad.Pricing.Brands.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Brands.Infrastructure.Persistence.Repositories;

public sealed class BrandRepository(BrandsDbContext context) : IBrandRepository
{
    public async Task<Brand?> GetByIdAsync(
        long id,
        bool includeConfigs = false,
        bool includeStores = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Brands.AsQueryable();

        if (includeConfigs)
            query = query.Include(x => x.Configs);

        if (includeStores)
            query = query.Include(x => x.Stores);

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Brand?> GetByPublicIdAsync(
        Guid publicId,
        bool includeConfigs = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Brands.AsQueryable();

        if (includeConfigs)
            query = query.Include(x => x.Configs);

        return await query.FirstOrDefaultAsync(x => x.PublicId == publicId, cancellationToken);
    }

    public async Task<Brand?> GetByCodeAsync(
        string code,
        bool includeConfigs = false,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        var query = context.Brands.AsQueryable();

        if (includeConfigs)
            query = query.Include(x => x.Configs);

        return await query.FirstOrDefaultAsync(x => x.Code == normalizedCode, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Brand>> GetAllAsync(
        bool includeConfigs = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Brands.AsNoTracking();

        if (includeConfigs)
            query = query.Include(x => x.Configs);

        return await query
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await context.Brands
            .AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        long? excludeBrandId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return await context.Brands
            .AnyAsync(x => x.Code == normalizedCode && (!excludeBrandId.HasValue || x.Id != excludeBrandId.Value), cancellationToken);
    }

    public async Task<long> AddAsync(
        Brand brand,
        CancellationToken cancellationToken = default)
    {
        await context.Brands.AddAsync(brand, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return brand.Id;
    }

    public async Task UpdateAsync(
        Brand brand,
        CancellationToken cancellationToken = default)
    {
        context.Brands.Update(brand);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Brand brand,
        CancellationToken cancellationToken = default)
    {
        context.Brands.Update(brand);
        await context.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<Brand> GetQueryable()
    {
        return context.Brands.AsQueryable();
    }
}
