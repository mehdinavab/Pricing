using Kahrad.Pricing.Guarantees.Application.Abstractions.Persistence;
using Kahrad.Pricing.Guarantees.Domain.Entities;
using Kahrad.Pricing.Guarantees.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Guarantees.Infrastructure.Persistence.Repositories;

public sealed class GuaranteeCertificateRepository(GuaranteesDbContext context) : IGuaranteeCertificateRepository
{
    public async Task<GuaranteeCertificate?> GetByIdAsync(
        long id,
        bool includeGuarantee = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.GuaranteeCertificates.AsQueryable();

        if (includeGuarantee)
        {
            query = query
                .Include(x => x.Guarantee)
                    .ThenInclude(g => g.Vendor);
        }

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<GuaranteeCertificate?> GetByPublicIdAsync(
        Guid publicId,
        bool includeGuarantee = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.GuaranteeCertificates.AsQueryable();

        if (includeGuarantee)
        {
            query = query
                .Include(x => x.Guarantee)
                    .ThenInclude(g => g.Vendor);
        }

        return await query.FirstOrDefaultAsync(x => x.PublicId == publicId, cancellationToken);
    }

    public async Task<GuaranteeCertificate?> GetByCertificateNumberAsync(
        string certificateNumber,
        bool includeGuarantee = false,
        CancellationToken cancellationToken = default)
    {
        var normalized = certificateNumber.Trim().ToUpperInvariant();
        var query = context.GuaranteeCertificates.AsQueryable();

        if (includeGuarantee)
        {
            query = query
                .Include(x => x.Guarantee)
                    .ThenInclude(g => g.Vendor);
        }

        return await query.FirstOrDefaultAsync(x => x.CertificateNumber == normalized, cancellationToken);
    }

    public async Task<IReadOnlyCollection<GuaranteeCertificate>> GetByProductIdAsync(
        long productId,
        CancellationToken cancellationToken = default)
    {
        return await context.GuaranteeCertificates
            .AsNoTracking()
            .Include(x => x.Guarantee)
                .ThenInclude(g => g.Vendor)
            .Where(x => x.ProductId == productId)
            .OrderByDescending(x => x.PurchaseDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await context.GuaranteeCertificates
            .AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCertificateNumberAsync(
        string certificateNumber,
        long? excludeCertificateId = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = certificateNumber.Trim().ToUpperInvariant();
        return await context.GuaranteeCertificates
            .AnyAsync(
                x => x.CertificateNumber == normalized &&
                     (!excludeCertificateId.HasValue || x.Id != excludeCertificateId.Value),
                cancellationToken);
    }

    public async Task<long> AddAsync(
        GuaranteeCertificate certificate,
        CancellationToken cancellationToken = default)
    {
        await context.GuaranteeCertificates.AddAsync(certificate, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return certificate.Id;
    }

    public async Task UpdateAsync(
        GuaranteeCertificate certificate,
        CancellationToken cancellationToken = default)
    {
        context.GuaranteeCertificates.Update(certificate);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        GuaranteeCertificate certificate,
        CancellationToken cancellationToken = default)
    {
        context.GuaranteeCertificates.Update(certificate);
        await context.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<GuaranteeCertificate> GetQueryable()
    {
        return context.GuaranteeCertificates.AsQueryable();
    }
}
