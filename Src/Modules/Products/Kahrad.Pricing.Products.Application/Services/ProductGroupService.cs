using AutoMapper;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Products.Application.Abstractions.Persistence;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.ProductGroups;
using Kahrad.Pricing.Products.Application.Exceptions;
using Kahrad.Pricing.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace Kahrad.Pricing.Products.Application.Services;

public sealed class ProductGroupService(
    IProductGroupRepository productGroupRepository,
    IMapper mapper,
    ISieveProcessor sieveProcessor) : IProductGroupService
{
    public async Task<ProductGroupDto?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var group = await productGroupRepository.GetByIdAsync(
            id,
            includeProducts: false,
            cancellationToken);

        return group is null ? null : mapper.Map<ProductGroupDto>(group);
    }

    public async Task<ProductGroupDto?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var group = await productGroupRepository.GetByCodeAsync(
            code,
            includeProducts: false,
            cancellationToken);

        return group is null ? null : mapper.Map<ProductGroupDto>(group);
    }

    public async Task<IReadOnlyCollection<ProductGroupDto>> GetAllAsync(
        bool onlyActive = false,
        CancellationToken cancellationToken = default)
    {
        var groups = await productGroupRepository.GetAllAsync(onlyActive, cancellationToken);
        return mapper.Map<IReadOnlyCollection<ProductGroupDto>>(groups);
    }

    public async Task<BaseResponsePaginated<ProductGroupDto>> GetPagedAsync(
        ProductGroupQueryDto filter,
        CancellationToken cancellationToken = default)
    {
        EnsureStableSort(filter);

        var query = productGroupRepository.GetQueryable().AsNoTracking();

        var totalCount = await sieveProcessor
            .Apply(
                filter,
                query,
                applyFiltering: true,
                applySorting: false,
                applyPagination: false)
            .CountAsync(cancellationToken);

        var groups = await sieveProcessor
            .Apply(
                filter,
                query,
                applyFiltering: true,
                applySorting: true,
                applyPagination: true)
            .ToListAsync(cancellationToken);

        var data = mapper.Map<List<ProductGroupDto>>(groups);

        return BaseResponsePaginated<ProductGroupDto>.Success(
            data,
            filter.Page ?? 1,
            filter.PageSize ?? 20,
            totalCount);
    }

    public async Task<long> CreateAsync(
        CreateProductGroupRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        if (await productGroupRepository.ExistsByCodeAsync(request.Code, null, cancellationToken))
        {
            throw new ProductOperationException(
                $"A product group with the code '{request.Code}' already exists.");
        }

        var utcNow = DateTime.UtcNow;
        var group = new ProductGroup(
            request.Name,
            request.Code,
            request.PriceEffectPercentage,
            request.Description,
            request.IsActive);

        group.SetCreated(currentUserId, utcNow);

        await productGroupRepository.AddAsync(group, cancellationToken);
        return group.Id;
    }

    public async Task UpdateAsync(
        long id,
        UpdateProductGroupRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var group = await productGroupRepository.GetByIdAsync(
            id,
            includeProducts: false,
            cancellationToken);

        if (group is null)
            throw new ProductGroupNotFoundException(id);

        if (await productGroupRepository.ExistsByCodeAsync(request.Code, id, cancellationToken))
        {
            throw new ProductOperationException(
                $"Another product group with the code '{request.Code}' already exists.");
        }

        group.Update(
            request.Name,
            request.Code,
            request.PriceEffectPercentage,
            request.Description,
            DateTime.UtcNow,
            currentUserId);

        await productGroupRepository.UpdateAsync(group, cancellationToken);
    }

    public async Task DeleteAsync(
        long id,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var group = await productGroupRepository.GetByIdAsync(
            id,
            includeProducts: false,
            cancellationToken);

        if (group is null)
            throw new ProductGroupNotFoundException(id);

        group.SetDeleted(currentUserId, DateTime.UtcNow);
        await productGroupRepository.DeleteAsync(group, cancellationToken);
    }

    public async Task ChangeStatusAsync(
        long id,
        bool isActive,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var group = await productGroupRepository.GetByIdAsync(
            id,
            includeProducts: false,
            cancellationToken);

        if (group is null)
            throw new ProductGroupNotFoundException(id);

        var utcNow = DateTime.UtcNow;
        if (isActive)
            group.Activate(utcNow, currentUserId);
        else
            group.Deactivate(utcNow, currentUserId);

        await productGroupRepository.UpdateAsync(group, cancellationToken);
    }

    private static void EnsureStableSort(ProductGroupQueryDto filter)
    {
        var sortFields = filter.Sorts?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.TrimStart('+', '-'));

        if (sortFields?.Contains("Id", StringComparer.OrdinalIgnoreCase) == true)
            return;

        filter.Sorts = string.IsNullOrWhiteSpace(filter.Sorts)
            ? "Name,Id"
            : $"{filter.Sorts},Id";
    }
}
