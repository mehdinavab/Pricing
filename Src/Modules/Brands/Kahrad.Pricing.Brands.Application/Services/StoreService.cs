using AutoMapper;
using Kahrad.Pricing.Brands.Application.Abstractions.Persistence;
using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Brands.Application.DTOs.Stores;
using Kahrad.Pricing.Brands.Application.Exceptions;
using Kahrad.Pricing.Brands.Domain.Entities;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace Kahrad.Pricing.Brands.Application.Services;

public sealed class StoreService(
    IStoreRepository storeRepository,
    IBrandRepository brandRepository,
    IMapper mapper,
    ISieveProcessor sieveProcessor) : IStoreService
{
    public async Task<StoreDto?> GetByIdAsync(
        long storeId,
        CancellationToken cancellationToken = default)
    {
        var store = await storeRepository.GetByIdAsync(
            storeId,
            includeBrand: true,
            cancellationToken);

        return store is null ? null : mapper.Map<StoreDto>(store);
    }

    public async Task<StoreDto?> GetByCodeAsync(
        long brandId,
        string code,
        CancellationToken cancellationToken = default)
    {
        var store = await storeRepository.GetByCodeAsync(
            brandId,
            code,
            includeBrand: true,
            cancellationToken);

        return store is null ? null : mapper.Map<StoreDto>(store);
    }

    public async Task<IReadOnlyCollection<StoreDto>> GetByBrandIdAsync(
        long brandId,
        CancellationToken cancellationToken = default)
    {
        if (!await brandRepository.ExistsAsync(brandId, cancellationToken))
            throw new BrandNotFoundException(brandId);

        var stores = await storeRepository.GetByBrandIdAsync(brandId, cancellationToken);
        return mapper.Map<IReadOnlyCollection<StoreDto>>(stores);
    }

    public async Task<BaseResponsePaginated<StoreDto>> GetPagedAsync(
        StoreQueryDto filter,
        CancellationToken cancellationToken = default)
    {
        EnsureStableSort(filter);

        var query = storeRepository.GetQueryable().AsNoTracking();

        if (filter.BrandId.HasValue)
        {
            query = query.Where(x => x.BrandId == filter.BrandId.Value);
        }

        var totalCount = await sieveProcessor
            .Apply(
                filter,
                query,
                applyFiltering: true,
                applySorting: false,
                applyPagination: false)
            .CountAsync(cancellationToken);

        var stores = await sieveProcessor
            .Apply(
                filter,
                query,
                applyFiltering: true,
                applySorting: true,
                applyPagination: true)
            .Include(x => x.Brand)
            .ToListAsync(cancellationToken);

        var data = mapper.Map<List<StoreDto>>(stores);

        return BaseResponsePaginated<StoreDto>.Success(
            data,
            filter.Page ?? 1,
            filter.PageSize ?? 20,
            totalCount);
    }

    public async Task<long> CreateAsync(
        CreateStoreRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        if (!await brandRepository.ExistsAsync(request.BrandId, cancellationToken))
            throw new BrandNotFoundException(request.BrandId);

        if (await storeRepository.ExistsByCodeAsync(request.BrandId, request.Code, null, cancellationToken))
        {
            throw new BrandOperationException(
                $"A store with code '{request.Code}' already exists for Brand '{request.BrandId}'.");
        }

        var utcNow = DateTime.UtcNow;
        var store = new Store(
            request.BrandId,
            request.Name,
            request.Code,
            request.OwnerName,
            request.Address,
            request.OwnerPhoneNumber,
            request.OwnerNationalCode,
            request.OwnerEmail,
            request.PictureUrl,
            request.City,
            request.Province,
            request.PostalCode,
            request.Phone,
            request.PriceAdjustmentType,
            request.PriceAdjustmentPercentage,
            request.Description,
            request.IsActive);

        store.SetCreated(currentUserId, utcNow);

        await storeRepository.AddAsync(store, cancellationToken);
        return store.Id;
    }

    public async Task UpdateAsync(
        long storeId,
        UpdateStoreRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var store = await storeRepository.GetByIdAsync(
            storeId,
            includeBrand: false,
            cancellationToken);

        if (store is null)
            throw new StoreNotFoundException(storeId);

        if (await storeRepository.ExistsByCodeAsync(store.BrandId, request.Code, storeId, cancellationToken))
        {
            throw new BrandOperationException(
                $"Another store with code '{request.Code}' already exists for Brand '{store.BrandId}'.");
        }

        store.Update(
            request.Name,
            request.Code,
            request.OwnerName,
            request.Address,
            request.OwnerPhoneNumber,
            request.OwnerNationalCode,
            request.OwnerEmail,
            request.PictureUrl,
            request.City,
            request.Province,
            request.PostalCode,
            request.Phone,
            request.Description,
            DateTime.UtcNow,
            currentUserId);

        await storeRepository.UpdateAsync(store, cancellationToken);
    }

    public async Task DeleteAsync(
        long storeId,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var store = await storeRepository.GetByIdAsync(
            storeId,
            includeBrand: false,
            cancellationToken);

        if (store is null)
            throw new StoreNotFoundException(storeId);

        store.SetDeleted(currentUserId, DateTime.UtcNow);
        await storeRepository.DeleteAsync(store, cancellationToken);
    }

    public async Task ChangeStatusAsync(
        long storeId,
        bool isActive,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var store = await storeRepository.GetByIdAsync(
            storeId,
            includeBrand: false,
            cancellationToken);

        if (store is null)
            throw new StoreNotFoundException(storeId);

        var utcNow = DateTime.UtcNow;
        if (isActive)
            store.Activate(utcNow, currentUserId);
        else
            store.Deactivate(utcNow, currentUserId);

        await storeRepository.UpdateAsync(store, cancellationToken);
    }

    public async Task UpdatePriceAdjustmentAsync(
        long storeId,
        UpdateStorePriceAdjustmentRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var store = await storeRepository.GetByIdAsync(
            storeId,
            includeBrand: false,
            cancellationToken);

        if (store is null)
            throw new StoreNotFoundException(storeId);

        store.SetPriceAdjustment(
            request.Type,
            request.Percentage,
            DateTime.UtcNow,
            currentUserId);

        await storeRepository.UpdateAsync(store, cancellationToken);
    }

    private static void EnsureStableSort(StoreQueryDto filter)
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
