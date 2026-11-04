using Kahrad.Pricing.Brands.Application.DTOs.BrandConfigs;
using Kahrad.Pricing.Brands.Application.DTOs.Brands;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;

namespace Kahrad.Pricing.Brands.Application.Abstractions.Services;

public interface IBrandService
{
    Task<BrandDto?> GetByIdAsync(
        long brandId,
        CancellationToken cancellationToken = default);

    Task<BrandDetailDto?> GetDetailByIdAsync(
        long brandId,
        CancellationToken cancellationToken = default);

    Task<BrandDto?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<BrandDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<BaseResponsePaginated<BrandDto>> GetPagedAsync(
        BrandQueryDto filter,
        CancellationToken cancellationToken = default);

    Task<long> CreateAsync(
        CreateBrandRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        long brandId,
        UpdateBrandRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        long brandId,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task ChangeStatusAsync(
        long brandId,
        bool isActive,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task SetActivePriceVersionAsync(
        long brandId,
        int version,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<BrandConfigDto>> GetConfigsAsync(
        long brandId,
        CancellationToken cancellationToken = default);

    Task SetConfigsAsync(
        long brandId,
        IEnumerable<SetBrandConfigRequest> configs,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task DeleteConfigAsync(
        long brandId,
        string key,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);
}
