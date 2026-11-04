using Kahrad.Pricing.Brands.Domain.Entities;

namespace Kahrad.Pricing.Brands.Application.Abstractions.Persistence;

public interface IBrandConfigRepository
{
    Task<IReadOnlyCollection<BrandConfig>> GetByBrandIdAsync(
        long brandId,
        CancellationToken cancellationToken = default);

    Task<BrandConfig?> GetByKeyAsync(
        long brandId,
        string key,
        CancellationToken cancellationToken = default);

    Task SetConfigsAsync(
        long brandId,
        IEnumerable<BrandConfig> configs,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        long brandId,
        string key,
        CancellationToken cancellationToken = default);
}
