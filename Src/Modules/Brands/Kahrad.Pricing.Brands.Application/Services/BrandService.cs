using AutoMapper;
using Kahrad.Pricing.Brands.Application.Abstractions.Persistence;
using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Brands.Application.DTOs.BrandConfigs;
using Kahrad.Pricing.Brands.Application.DTOs.Brands;
using Kahrad.Pricing.Brands.Application.Exceptions;
using Kahrad.Pricing.Brands.Domain.Entities;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace Kahrad.Pricing.Brands.Application.Services;

public sealed class BrandService(
    IBrandRepository brandRepository,
    IBrandConfigRepository brandConfigRepository,
    IMapper mapper,
    ISieveProcessor sieveProcessor) : IBrandService
{
    public async Task<BrandDto?> GetByIdAsync(
        long brandId,
        CancellationToken cancellationToken = default)
    {
        var brand = await brandRepository.GetByIdAsync(
            brandId,
            includeConfigs: false,
            includeStores: false,
            cancellationToken);

        return brand is null ? null : mapper.Map<BrandDto>(brand);
    }

    public async Task<BrandDetailDto?> GetDetailByIdAsync(
        long brandId,
        CancellationToken cancellationToken = default)
    {
        var brand = await brandRepository.GetByIdAsync(
            brandId,
            includeConfigs: true,
            includeStores: true,
            cancellationToken);

        return brand is null ? null : mapper.Map<BrandDetailDto>(brand);
    }

    public async Task<BrandDto?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var brand = await brandRepository.GetByCodeAsync(
            code,
            includeConfigs: false,
            cancellationToken);

        return brand is null ? null : mapper.Map<BrandDto>(brand);
    }

    public async Task<IReadOnlyCollection<BrandDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var brands = await brandRepository.GetAllAsync(
            includeConfigs: false,
            cancellationToken);

        return mapper.Map<IReadOnlyCollection<BrandDto>>(brands);
    }

    public async Task<BaseResponsePaginated<BrandDto>> GetPagedAsync(
        BrandQueryDto filter,
        CancellationToken cancellationToken = default)
    {
        EnsureStableSort(filter);

        var query = brandRepository.GetQueryable().AsNoTracking();

        var totalCount = await sieveProcessor
            .Apply(
                filter,
                query,
                applyFiltering: true,
                applySorting: false,
                applyPagination: false)
            .CountAsync(cancellationToken);

        var brands = await sieveProcessor
            .Apply(
                filter,
                query,
                applyFiltering: true,
                applySorting: true,
                applyPagination: true)
            .ToListAsync(cancellationToken);

        var data = mapper.Map<List<BrandDto>>(brands);

        return BaseResponsePaginated<BrandDto>.Success(
            data,
            filter.Page ?? 1,
            filter.PageSize ?? 20,
            totalCount);
    }

    public async Task<long> CreateAsync(
        CreateBrandRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        if (await brandRepository.ExistsByCodeAsync(request.Code, null, cancellationToken))
        {
            throw new BrandOperationException(
                $"A brand with the code '{request.Code}' already exists.");
        }

        var utcNow = DateTime.UtcNow;
        var brand = new Brand(
            request.Name,
            request.Code,
            request.Description,
            request.LogoUrl,
            request.Website,
            request.SupportPhone,
            request.SupportEmail,
            request.InitialActivePriceVersion ?? 1,
            request.IsActive);

        brand.SetCreated(currentUserId, utcNow);

        await brandRepository.AddAsync(brand, cancellationToken);
        return brand.Id;
    }

    public async Task UpdateAsync(
        long brandId,
        UpdateBrandRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var brand = await brandRepository.GetByIdAsync(
            brandId,
            includeConfigs: false,
            includeStores: false,
            cancellationToken);

        if (brand is null)
            throw new BrandNotFoundException(brandId);

        if (await brandRepository.ExistsByCodeAsync(request.Code, brandId, cancellationToken))
        {
            throw new BrandOperationException(
                $"Another brand with the code '{request.Code}' already exists.");
        }

        brand.Update(
            request.Name,
            request.Code,
            request.Description,
            request.LogoUrl,
            request.Website,
            request.SupportPhone,
            request.SupportEmail,
            DateTime.UtcNow,
            currentUserId);

        await brandRepository.UpdateAsync(brand, cancellationToken);
    }

    public async Task DeleteAsync(
        long brandId,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var brand = await brandRepository.GetByIdAsync(
            brandId,
            includeConfigs: false,
            includeStores: false,
            cancellationToken);

        if (brand is null)
            throw new BrandNotFoundException(brandId);

        brand.SetDeleted(currentUserId, DateTime.UtcNow);
        await brandRepository.DeleteAsync(brand, cancellationToken);
    }

    public async Task ChangeStatusAsync(
        long brandId,
        bool isActive,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var brand = await brandRepository.GetByIdAsync(
            brandId,
            includeConfigs: false,
            includeStores: false,
            cancellationToken);

        if (brand is null)
            throw new BrandNotFoundException(brandId);

        var utcNow = DateTime.UtcNow;
        if (isActive)
            brand.Activate(utcNow, currentUserId);
        else
            brand.Deactivate(utcNow, currentUserId);

        await brandRepository.UpdateAsync(brand, cancellationToken);
    }

    public async Task SetActivePriceVersionAsync(
        long brandId,
        int version,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var brand = await brandRepository.GetByIdAsync(
            brandId,
            includeConfigs: false,
            includeStores: false,
            cancellationToken);

        if (brand is null)
            throw new BrandNotFoundException(brandId);

        brand.SetActivePriceVersion(version, DateTime.UtcNow, currentUserId);
        await brandRepository.UpdateAsync(brand, cancellationToken);
    }

    public async Task<IReadOnlyCollection<BrandConfigDto>> GetConfigsAsync(
        long brandId,
        CancellationToken cancellationToken = default)
    {
        if (!await brandRepository.ExistsAsync(brandId, cancellationToken))
            throw new BrandNotFoundException(brandId);

        var configs = await brandConfigRepository.GetByBrandIdAsync(brandId, cancellationToken);
        return mapper.Map<IReadOnlyCollection<BrandConfigDto>>(configs);
    }

    public async Task SetConfigsAsync(
        long brandId,
        IEnumerable<SetBrandConfigRequest> configs,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        if (!await brandRepository.ExistsAsync(brandId, cancellationToken))
            throw new BrandNotFoundException(brandId);

        var utcNow = DateTime.UtcNow;
        var configEntities = configs.Select(c =>
        {
            var config = new BrandConfig(
                brandId,
                c.Key,
                c.DisplayName,
                c.Value,
                c.DataType,
                c.Category,
                c.Description,
                c.IsActive);
            config.SetCreated(currentUserId, utcNow);
            return config;
        }).ToList();

        await brandConfigRepository.SetConfigsAsync(brandId, configEntities, cancellationToken);
    }

    public async Task DeleteConfigAsync(
        long brandId,
        string key,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        if (!await brandRepository.ExistsAsync(brandId, cancellationToken))
            throw new BrandNotFoundException(brandId);

        await brandConfigRepository.DeleteAsync(brandId, key, cancellationToken);
    }

    private static void EnsureStableSort(BrandQueryDto filter)
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
