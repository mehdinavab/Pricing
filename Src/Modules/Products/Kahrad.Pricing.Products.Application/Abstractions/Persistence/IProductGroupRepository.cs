using Kahrad.Pricing.Products.Domain.Entities;

namespace Kahrad.Pricing.Products.Application.Abstractions.Persistence;

public interface IProductGroupRepository
{
    Task<ProductGroup?> GetByIdAsync(
        long id,
        bool includeProducts = false,
        CancellationToken cancellationToken = default);

    Task<ProductGroup?> GetByPublicIdAsync(
        Guid publicId,
        bool includeProducts = false,
        CancellationToken cancellationToken = default);

    Task<ProductGroup?> GetByCodeAsync(
        string code,
        bool includeProducts = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ProductGroup>> GetAllAsync(
        bool onlyActive = false,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        long? excludeGroupId = null,
        CancellationToken cancellationToken = default);

    Task<long> AddAsync(
        ProductGroup group,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        ProductGroup group,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        ProductGroup group,
        CancellationToken cancellationToken = default);

    IQueryable<ProductGroup> GetQueryable();
}
