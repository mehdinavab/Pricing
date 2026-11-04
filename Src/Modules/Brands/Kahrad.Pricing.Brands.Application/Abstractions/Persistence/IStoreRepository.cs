using Kahrad.Pricing.Brands.Domain.Entities;

namespace Kahrad.Pricing.Brands.Application.Abstractions.Persistence;

public interface IStoreRepository
{
    Task<Store?> GetByIdAsync(
        long id,
        bool includeBrand = false,
        CancellationToken cancellationToken = default);

    Task<Store?> GetByPublicIdAsync(
        Guid publicId,
        bool includeBrand = false,
        CancellationToken cancellationToken = default);

    Task<Store?> GetByCodeAsync(
        long brandId,
        string code,
        bool includeBrand = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Store>> GetByBrandIdAsync(
        long brandId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        long brandId,
        string code,
        long? excludeStoreId = null,
        CancellationToken cancellationToken = default);

    Task<long> AddAsync(
        Store store,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Store store,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Store store,
        CancellationToken cancellationToken = default);

    IQueryable<Store> GetQueryable();
}
