using Kahrad.Pricing.Guarantees.Domain.Entities;

namespace Kahrad.Pricing.Guarantees.Application.Abstractions.Persistence;

public interface IVendorRepository
{
    Task<Vendor?> GetByIdAsync(
        long id,
        bool includeGuarantees = false,
        CancellationToken cancellationToken = default);

    Task<Vendor?> GetByPublicIdAsync(
        Guid publicId,
        bool includeGuarantees = false,
        CancellationToken cancellationToken = default);

    Task<Vendor?> GetByCodeAsync(
        string code,
        bool includeGuarantees = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Vendor>> GetAllAsync(
        bool includeGuarantees = false,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        long? excludeVendorId = null,
        CancellationToken cancellationToken = default);

    Task<long> AddAsync(
        Vendor vendor,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Vendor vendor,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Vendor vendor,
        CancellationToken cancellationToken = default);

    IQueryable<Vendor> GetQueryable();
}
