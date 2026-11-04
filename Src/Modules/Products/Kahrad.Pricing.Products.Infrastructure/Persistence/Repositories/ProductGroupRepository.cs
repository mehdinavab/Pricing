using Kahrad.Pricing.Products.Application.Abstractions.Persistence;
using Kahrad.Pricing.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Products.Infrastructure.Persistence.Repositories;

public sealed class ProductGroupRepository(ProductsDbContext context) : IProductGroupRepository
{
    public async Task<ProductGroup?> GetByIdAsync(
        long id,
        bool includeProducts = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.ProductGroups.AsQueryable();

        if (includeProducts)
            query = query.Include(x => x.Products);

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<ProductGroup?> GetByPublicIdAsync(
        Guid publicId,
        bool includeProducts = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.ProductGroups.AsQueryable();

        if (includeProducts)
            query = query.Include(x => x.Products);

        return await query.FirstOrDefaultAsync(x => x.PublicId == publicId, cancellationToken);
    }

    public async Task<ProductGroup?> GetByCodeAsync(
        string code,
        bool includeProducts = false,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        var query = context.ProductGroups.AsQueryable();

        if (includeProducts)
            query = query.Include(x => x.Products);

        return await query.FirstOrDefaultAsync(x => x.Code == normalizedCode, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ProductGroup>> GetAllAsync(
        bool onlyActive = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.ProductGroups.AsNoTracking();

        if (onlyActive)
            query = query.Where(x => x.IsActive);

        return await query
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await context.ProductGroups
            .AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        long? excludeGroupId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return await context.ProductGroups
            .AnyAsync(
                x => x.Code == normalizedCode &&
                     (!excludeGroupId.HasValue || x.Id != excludeGroupId.Value),
                cancellationToken);
    }

    public async Task<long> AddAsync(
        ProductGroup group,
        CancellationToken cancellationToken = default)
    {
        await context.ProductGroups.AddAsync(group, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return group.Id;
    }

    public async Task UpdateAsync(
        ProductGroup group,
        CancellationToken cancellationToken = default)
    {
        context.ProductGroups.Update(group);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        ProductGroup group,
        CancellationToken cancellationToken = default)
    {
        context.ProductGroups.Update(group);
        await context.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<ProductGroup> GetQueryable()
    {
        return context.ProductGroups.AsQueryable();
    }
}
