using Kahrad.Pricing.Brands.Domain.Entities;

namespace Kahrad.Pricing.Brands.Application.Abstractions.Persistence;

public interface IBrandRepository
{
    Task<Brand?> GetByIdAsync(
        long id,
        bool includeConfigs = false,
        bool includeStores = false,
        CancellationToken cancellationToken = default);

    Task<Brand?> GetByPublicIdAsync(
        Guid publicId,
        bool includeConfigs = false,
        CancellationToken cancellationToken = default);

    Task<Brand?> GetByCodeAsync(
        string code,
        bool includeConfigs = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Brand>> GetAllAsync(
        bool includeConfigs = false,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        long? excludeBrandId = null,
        CancellationToken cancellationToken = default);

    Task<long> AddAsync(
        Brand brand,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Brand brand,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Brand brand,
        CancellationToken cancellationToken = default);

    IQueryable<Brand> GetQueryable();
}
