using Kahrad.Pricing.Guarantees.Application.Abstractions.Persistence;
using Kahrad.Pricing.Guarantees.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Guarantees.Infrastructure.Persistence.Repositories;

public sealed class GuaranteeRepository(GuaranteesDbContext context) : IGuaranteeRepository
{
    public async Task<Guarantee?> GetByIdAsync(
        long id,
        bool includeVendor = false,
        bool includeCertificates = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Guarantees.AsQueryable();

        if (includeVendor)
            query = query.Include(x => x.Vendor);

        if (includeCertificates)
            query = query.Include(x => x.Certificates);

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Guarantee?> GetByPublicIdAsync(
        Guid publicId,
        bool includeVendor = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Guarantees.AsQueryable();

        if (includeVendor)
            query = query.Include(x => x.Vendor);

        return await query.FirstOrDefaultAsync(x => x.PublicId == publicId, cancellationToken);
    }

    public async Task<Guarantee?> GetByCodeAsync(
        long vendorId,
        string code,
        bool includeVendor = false,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        var query = context.Guarantees.AsQueryable();

        if (includeVendor)
            query = query.Include(x => x.Vendor);

        return await query.FirstOrDefaultAsync(
            x => x.VendorId == vendorId && x.Code == normalizedCode,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Guarantee>> GetByVendorIdAsync(
        long vendorId,
        CancellationToken cancellationToken = default)
    {
        return await context.Guarantees
            .AsNoTracking()
            .Include(x => x.Vendor)
            .Where(x => x.VendorId == vendorId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await context.Guarantees
            .AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        long vendorId,
        string code,
        long? excludeGuaranteeId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return await context.Guarantees
            .AnyAsync(
                x => x.VendorId == vendorId &&
                     x.Code == normalizedCode &&
                     (!excludeGuaranteeId.HasValue || x.Id != excludeGuaranteeId.Value),
                cancellationToken);
    }

    public async Task<long> AddAsync(
        Guarantee guarantee,
        CancellationToken cancellationToken = default)
    {
        await context.Guarantees.AddAsync(guarantee, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return guarantee.Id;
    }

    public async Task UpdateAsync(
        Guarantee guarantee,
        CancellationToken cancellationToken = default)
    {
        context.Guarantees.Update(guarantee);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Guarantee guarantee,
        CancellationToken cancellationToken = default)
    {
        context.Guarantees.Update(guarantee);
        await context.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<Guarantee> GetQueryable()
    {
        return context.Guarantees.AsQueryable();
    }
}
