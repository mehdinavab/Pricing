using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Products.Application.DTOs.ProductGroups;

namespace Kahrad.Pricing.Products.Application.Abstractions.Services;

public interface IProductGroupService
{
    Task<ProductGroupDto?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<ProductGroupDto?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ProductGroupDto>> GetAllAsync(
        bool onlyActive = false,
        CancellationToken cancellationToken = default);

    Task<BaseResponsePaginated<ProductGroupDto>> GetPagedAsync(
        ProductGroupQueryDto filter,
        CancellationToken cancellationToken = default);

    Task<long> CreateAsync(
        CreateProductGroupRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        long id,
        UpdateProductGroupRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        long id,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task ChangeStatusAsync(
        long id,
        bool isActive,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);
}
