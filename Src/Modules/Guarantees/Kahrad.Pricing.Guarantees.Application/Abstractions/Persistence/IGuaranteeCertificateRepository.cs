using Kahrad.Pricing.Guarantees.Domain.Entities;

namespace Kahrad.Pricing.Guarantees.Application.Abstractions.Persistence;

public interface IGuaranteeCertificateRepository
{
    Task<GuaranteeCertificate?> GetByIdAsync(
        long id,
        bool includeGuarantee = false,
        CancellationToken cancellationToken = default);

    Task<GuaranteeCertificate?> GetByPublicIdAsync(
        Guid publicId,
        bool includeGuarantee = false,
        CancellationToken cancellationToken = default);

    Task<GuaranteeCertificate?> GetByCertificateNumberAsync(
        string certificateNumber,
        bool includeGuarantee = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<GuaranteeCertificate>> GetByProductIdAsync(
        long productId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCertificateNumberAsync(
        string certificateNumber,
        long? excludeCertificateId = null,
        CancellationToken cancellationToken = default);

    Task<long> AddAsync(
        GuaranteeCertificate certificate,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        GuaranteeCertificate certificate,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        GuaranteeCertificate certificate,
        CancellationToken cancellationToken = default);

    IQueryable<GuaranteeCertificate> GetQueryable();
}
