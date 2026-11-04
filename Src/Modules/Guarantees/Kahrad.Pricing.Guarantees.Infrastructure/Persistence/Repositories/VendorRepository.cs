using Kahrad.Pricing.Guarantees.Application.Abstractions.Persistence;
using Kahrad.Pricing.Guarantees.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Guarantees.Infrastructure.Persistence.Repositories;

public sealed class VendorRepository(GuaranteesDbContext context) : IVendorRepository
{
    public async Task<Vendor?> GetByIdAsync(
        long id,
        bool includeGuarantees = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Vendors.AsQueryable();

        if (includeGuarantees)
            query = query.Include(x => x.Guarantees);

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Vendor?> GetByPublicIdAsync(
        Guid publicId,
        bool includeGuarantees = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Vendors.AsQueryable();

        if (includeGuarantees)
            query = query.Include(x => x.Guarantees);

        return await query.FirstOrDefaultAsync(x => x.PublicId == publicId, cancellationToken);
    }

    public async Task<Vendor?> GetByCodeAsync(
        string code,
        bool includeGuarantees = false,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        var query = context.Vendors.AsQueryable();

        if (includeGuarantees)
            query = query.Include(x => x.Guarantees);

        return await query.FirstOrDefaultAsync(x => x.Code == normalizedCode, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Vendor>> GetAllAsync(
        bool includeGuarantees = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Vendors.AsNoTracking();

        if (includeGuarantees)
            query = query.Include(x => x.Guarantees);

        return await query
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await context.Vendors
            .AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        long? excludeVendorId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return await context.Vendors
            .AnyAsync(
                x => x.Code == normalizedCode &&
                     (!excludeVendorId.HasValue || x.Id != excludeVendorId.Value),
                cancellationToken);
    }

    public async Task<long> AddAsync(
        Vendor vendor,
        CancellationToken cancellationToken = default)
    {
        await context.Vendors.AddAsync(vendor, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return vendor.Id;
    }

    public async Task UpdateAsync(
        Vendor vendor,
        CancellationToken cancellationToken = default)
    {
        context.Vendors.Update(vendor);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Vendor vendor,
        CancellationToken cancellationToken = default)
    {
        context.Vendors.Update(vendor);
        await context.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<Vendor> GetQueryable()
    {
        return context.Vendors.AsQueryable();
    }
}
