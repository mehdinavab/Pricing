using Kahrad.Pricing.Guarantees.Domain.Entities;

namespace Kahrad.Pricing.Guarantees.Application.Abstractions.Persistence;

public interface IGuaranteeRepository
{
    Task<Guarantee?> GetByIdAsync(
        long id,
        bool includeVendor = false,
        bool includeCertificates = false,
        CancellationToken cancellationToken = default);

    Task<Guarantee?> GetByPublicIdAsync(
        Guid publicId,
        bool includeVendor = false,
        CancellationToken cancellationToken = default);

    Task<Guarantee?> GetByCodeAsync(
        long vendorId,
        string code,
        bool includeVendor = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Guarantee>> GetByVendorIdAsync(
        long vendorId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        long vendorId,
        string code,
        long? excludeGuaranteeId = null,
        CancellationToken cancellationToken = default);

    Task<long> AddAsync(
        Guarantee guarantee,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Guarantee guarantee,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guarantee guarantee,
        CancellationToken cancellationToken = default);

    IQueryable<Guarantee> GetQueryable();
}
