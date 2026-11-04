using Kahrad.Pricing.Brands.Application.DTOs.Stores;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;

namespace Kahrad.Pricing.Brands.Application.Abstractions.Services;

public interface IStoreService
{
    Task<StoreDto?> GetByIdAsync(
        long storeId,
        CancellationToken cancellationToken = default);

    Task<StoreDto?> GetByCodeAsync(
        long brandId,
        string code,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StoreDto>> GetByBrandIdAsync(
        long brandId,
        CancellationToken cancellationToken = default);

    Task<BaseResponsePaginated<StoreDto>> GetPagedAsync(
        StoreQueryDto filter,
        CancellationToken cancellationToken = default);

    Task<long> CreateAsync(
        CreateStoreRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        long storeId,
        UpdateStoreRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        long storeId,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task ChangeStatusAsync(
        long storeId,
        bool isActive,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task UpdatePriceAdjustmentAsync(
        long storeId,
        UpdateStorePriceAdjustmentRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);
}
