using AutoMapper;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Products.Application.Abstractions.Persistence;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.Features;
using Kahrad.Pricing.Products.Application.DTOs.Gallery;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Kahrad.Pricing.Products.Application.Exceptions;
using Kahrad.Pricing.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace Kahrad.Pricing.Products.Application.Services;

public sealed class ProductService(
    IProductRepository productRepository,
    IProductGroupRepository productGroupRepository,
    IMapper mapper,
    ISieveProcessor sieveProcessor) : IProductService
{
    // ==========================================
    // Product Queries
    // ==========================================

    public async Task<ProductDto?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(
            id,
            includeGroup: true,
            includeFeatures: false,
            includeGallery: false,
            cancellationToken);

        return product is null ? null : mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDetailDto?> GetDetailByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(
            id,
            includeGroup: true,
            includeFeatures: true,
            includeGallery: true,
            cancellationToken);

        return product is null ? null : mapper.Map<ProductDetailDto>(product);
    }

    public async Task<ProductDto?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByCodeAsync(
            code,
            includeGroup: true,
            includeFeatures: false,
            includeGallery: false,
            cancellationToken);

        return product is null ? null : mapper.Map<ProductDto>(product);
    }

    public async Task<IReadOnlyCollection<ProductDto>> GetAllAsync(
        long? groupId = null,
        long? brandId = null,
        bool onlyActive = false,
        CancellationToken cancellationToken = default)
    {
        var products = await productRepository.GetAllAsync(groupId, brandId, onlyActive, cancellationToken);
        return mapper.Map<IReadOnlyCollection<ProductDto>>(products);
    }

    public async Task<BaseResponsePaginated<ProductDto>> GetPagedAsync(
        ProductQueryDto filter,
        CancellationToken cancellationToken = default)
    {
        EnsureStableSort(filter);

        var query = productRepository.GetQueryable()
            .AsNoTracking()
            .Include(x => x.Group)
            .AsQueryable();

        if (filter.GroupId.HasValue)
            query = query.Where(x => x.GroupId == filter.GroupId.Value);

        if (filter.BrandId.HasValue)
            query = query.Where(x => x.BrandId == filter.BrandId.Value);

        if (filter.IsActive.HasValue)
            query = query.Where(x => x.IsActive == filter.IsActive.Value);

        var totalCount = await sieveProcessor
            .Apply(
                filter,
                query,
                applyFiltering: true,
                applySorting: false,
                applyPagination: false)
            .CountAsync(cancellationToken);

        var products = await sieveProcessor
            .Apply(
                filter,
                query,
                applyFiltering: true,
                applySorting: true,
                applyPagination: true)
            .ToListAsync(cancellationToken);

        var data = mapper.Map<List<ProductDto>>(products);

        return BaseResponsePaginated<ProductDto>.Success(
            data,
            filter.Page ?? 1,
            filter.PageSize ?? 20,
            totalCount);
    }

    // ==========================================
    // Product Mutations
    // ==========================================

    public async Task<long> CreateAsync(
        CreateProductRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        if (await productRepository.ExistsByCodeAsync(request.Code, null, cancellationToken))
        {
            throw new ProductOperationException(
                $"A product with the code '{request.Code}' already exists.");
        }

        if (!await productGroupRepository.ExistsAsync(request.GroupId, cancellationToken))
        {
            throw new ProductGroupNotFoundException(request.GroupId);
        }

        var utcNow = DateTime.UtcNow;
        var product = new Product(
            request.Name,
            request.Code,
            request.GroupId,
            request.BasePrice,
            request.BrandId,
            request.BeautyFactorPercentage,
            request.CommercialFactorPercentage,
            request.SalesFactorPercentage,
            request.Description,
            request.IsActive);

        product.SetCreated(currentUserId, utcNow);

        await productRepository.AddAsync(product, cancellationToken);
        return product.Id;
    }

    public async Task UpdateAsync(
        long id,
        UpdateProductRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(
            id,
            includeGroup: false,
            includeFeatures: false,
            includeGallery: false,
            cancellationToken);

        if (product is null)
            throw new ProductNotFoundException(id);

        if (await productRepository.ExistsByCodeAsync(request.Code, id, cancellationToken))
        {
            throw new ProductOperationException(
                $"Another product with the code '{request.Code}' already exists.");
        }

        if (!await productGroupRepository.ExistsAsync(request.GroupId, cancellationToken))
        {
            throw new ProductGroupNotFoundException(request.GroupId);
        }

        product.Update(
            request.Name,
            request.Code,
            request.GroupId,
            request.BrandId,
            request.BasePrice,
            request.Description,
            DateTime.UtcNow,
            currentUserId);

        await productRepository.UpdateAsync(product, cancellationToken);
    }

    public async Task DeleteAsync(
        long id,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(
            id,
            includeGroup: false,
            includeFeatures: false,
            includeGallery: false,
            cancellationToken);

        if (product is null)
            throw new ProductNotFoundException(id);

        product.SetDeleted(currentUserId, DateTime.UtcNow);
        await productRepository.DeleteAsync(product, cancellationToken);
    }

    public async Task ChangeStatusAsync(
        long id,
        bool isActive,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(
            id,
            includeGroup: false,
            includeFeatures: false,
            includeGallery: false,
            cancellationToken);

        if (product is null)
            throw new ProductNotFoundException(id);

        var utcNow = DateTime.UtcNow;
        if (isActive)
            product.Activate(utcNow, currentUserId);
        else
            product.Deactivate(utcNow, currentUserId);

        await productRepository.UpdateAsync(product, cancellationToken);
    }

    public async Task UpdatePricingFactorsAsync(
        long id,
        UpdateProductPricingFactorsRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(
            id,
            includeGroup: false,
            includeFeatures: false,
            includeGallery: false,
            cancellationToken);

        if (product is null)
            throw new ProductNotFoundException(id);

        product.UpdatePricingFactors(
            request.BeautyFactorPercentage,
            request.CommercialFactorPercentage,
            request.SalesFactorPercentage,
            DateTime.UtcNow,
            currentUserId);

        await productRepository.UpdateAsync(product, cancellationToken);
    }

    // ==========================================
    // Pricing Calculation
    // ==========================================

    public async Task<ProductPriceCalculationDto> CalculateEstimatedPriceAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(
            id,
            includeGroup: true,
            includeFeatures: true,
            includeGallery: false,
            cancellationToken);

        if (product is null)
            throw new ProductNotFoundException(id);

        var basePrice = product.BasePrice;

        // Group effect
        var groupPercentage = product.Group?.PriceEffectPercentage ?? 0;
        var groupAmount = Math.Round(basePrice * (groupPercentage / 100m), 4);

        // Beauty factor
        var beautyPercentage = product.BeautyFactorPercentage;
        var beautyAmount = Math.Round(basePrice * (beautyPercentage / 100m), 4);

        // Commercial factor (Bazargani)
        var commercialPercentage = product.CommercialFactorPercentage;
        var commercialAmount = Math.Round(basePrice * (commercialPercentage / 100m), 4);

        // Sales factor
        var salesPercentage = product.SalesFactorPercentage;
        var salesAmount = Math.Round(basePrice * (salesPercentage / 100m), 4);

        // Features effect (only features with HasPriceEffect == true)
        var priceEffectFeatures = product.Features.Where(f => f.HasPriceEffect && !f.IsDeleted).ToList();
        var featuresPercentageTotal = priceEffectFeatures.Sum(f => f.PriceEffectPercentage);
        var featuresPercentageAmount = Math.Round(basePrice * (featuresPercentageTotal / 100m), 4);
        var featuresFixedAmountTotal = priceEffectFeatures.Sum(f => f.PriceEffectAmount);

        var estimatedTotalPrice = basePrice
            + groupAmount
            + beautyAmount
            + commercialAmount
            + salesAmount
            + featuresPercentageAmount
            + featuresFixedAmountTotal;

        return new ProductPriceCalculationDto
        {
            ProductId = product.Id,
            ProductCode = product.Code,
            ProductName = product.Name,
            BasePrice = basePrice,
            GroupId = product.Group?.Id,
            GroupName = product.Group?.Name,
            GroupPriceEffectPercentage = groupPercentage,
            GroupPriceEffectAmount = groupAmount,
            BeautyFactorPercentage = beautyPercentage,
            BeautyFactorAmount = beautyAmount,
            CommercialFactorPercentage = commercialPercentage,
            CommercialFactorAmount = commercialAmount,
            SalesFactorPercentage = salesPercentage,
            SalesFactorAmount = salesAmount,
            FeaturesPercentageTotal = featuresPercentageTotal,
            FeaturesPercentageAmount = featuresPercentageAmount,
            FeaturesFixedAmountTotal = featuresFixedAmountTotal,
            EstimatedTotalPrice = estimatedTotalPrice
        };
    }

    // ==========================================
    // Feature Operations
    // ==========================================

    public async Task<IReadOnlyCollection<ProductFeatureDto>> GetFeaturesAsync(
        long productId,
        bool? hasPriceEffect = null,
        CancellationToken cancellationToken = default)
    {
        if (!await productRepository.ExistsAsync(productId, cancellationToken))
            throw new ProductNotFoundException(productId);

        var features = await productRepository.GetFeaturesByProductIdAsync(
            productId,
            hasPriceEffect,
            cancellationToken);

        return mapper.Map<IReadOnlyCollection<ProductFeatureDto>>(features);
    }

    public async Task<ProductFeatureDto?> GetFeatureByIdAsync(
        long featureId,
        CancellationToken cancellationToken = default)
    {
        var feature = await productRepository.GetFeatureByIdAsync(featureId, cancellationToken);
        return feature is null ? null : mapper.Map<ProductFeatureDto>(feature);
    }

    public async Task<long> AddFeatureAsync(
        long productId,
        AddProductFeatureRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        if (!await productRepository.ExistsAsync(productId, cancellationToken))
            throw new ProductNotFoundException(productId);

        var utcNow = DateTime.UtcNow;
        var feature = new ProductFeature(
            productId,
            request.Name,
            request.Value,
            request.HasPriceEffect,
            request.PriceEffectPercentage,
            request.PriceEffectAmount,
            request.DisplayOrder,
            request.Description);

        feature.SetCreated(currentUserId, utcNow);

        return await productRepository.AddFeatureAsync(feature, cancellationToken);
    }

    public async Task UpdateFeatureAsync(
        long featureId,
        UpdateProductFeatureRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var feature = await productRepository.GetFeatureByIdAsync(featureId, cancellationToken);
        if (feature is null)
            throw new ProductFeatureNotFoundException(featureId);

        feature.Update(
            request.Name,
            request.Value,
            request.HasPriceEffect,
            request.PriceEffectPercentage,
            request.PriceEffectAmount,
            request.DisplayOrder,
            request.Description,
            DateTime.UtcNow,
            currentUserId);

        await productRepository.UpdateFeatureAsync(feature, cancellationToken);
    }

    public async Task DeleteFeatureAsync(
        long featureId,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var feature = await productRepository.GetFeatureByIdAsync(featureId, cancellationToken);
        if (feature is null)
            throw new ProductFeatureNotFoundException(featureId);

        feature.SetDeleted(currentUserId, DateTime.UtcNow);
        await productRepository.DeleteFeatureAsync(feature, cancellationToken);
    }

    // ==========================================
    // Gallery Operations
    // ==========================================

    public async Task<IReadOnlyCollection<ProductImageDto>> GetGalleryAsync(
        long productId,
        CancellationToken cancellationToken = default)
    {
        if (!await productRepository.ExistsAsync(productId, cancellationToken))
            throw new ProductNotFoundException(productId);

        var images = await productRepository.GetGalleryByProductIdAsync(productId, cancellationToken);
        return mapper.Map<IReadOnlyCollection<ProductImageDto>>(images);
    }

    public async Task<ProductImageDto?> GetImageByIdAsync(
        long imageId,
        CancellationToken cancellationToken = default)
    {
        var image = await productRepository.GetImageByIdAsync(imageId, cancellationToken);
        return image is null ? null : mapper.Map<ProductImageDto>(image);
    }

    public async Task<long> AddImageAsync(
        long productId,
        AddProductImageRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        if (!await productRepository.ExistsAsync(productId, cancellationToken))
            throw new ProductNotFoundException(productId);

        if (request.IsPrimary)
        {
            await productRepository.ClearPrimaryImagesAsync(productId, null, cancellationToken);
        }

        var utcNow = DateTime.UtcNow;
        var image = new ProductImage(
            productId,
            request.ImageUrl,
            request.AltText,
            request.DisplayOrder,
            request.IsPrimary,
            request.Description);

        image.SetCreated(currentUserId, utcNow);

        return await productRepository.AddImageAsync(image, cancellationToken);
    }

    public async Task UpdateImageAsync(
        long imageId,
        UpdateProductImageRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var image = await productRepository.GetImageByIdAsync(imageId, cancellationToken);
        if (image is null)
            throw new ProductImageNotFoundException(imageId);

        image.Update(
            request.ImageUrl,
            request.AltText,
            request.DisplayOrder,
            request.Description,
            DateTime.UtcNow,
            currentUserId);

        await productRepository.UpdateImageAsync(image, cancellationToken);
    }

    public async Task DeleteImageAsync(
        long imageId,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var image = await productRepository.GetImageByIdAsync(imageId, cancellationToken);
        if (image is null)
            throw new ProductImageNotFoundException(imageId);

        image.SetDeleted(currentUserId, DateTime.UtcNow);
        await productRepository.DeleteImageAsync(image, cancellationToken);
    }

    public async Task SetPrimaryImageAsync(
        long productId,
        long imageId,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var image = await productRepository.GetImageByIdAsync(imageId, cancellationToken);
        if (image is null || image.ProductId != productId)
            throw new ProductImageNotFoundException(imageId);

        await productRepository.ClearPrimaryImagesAsync(productId, imageId, cancellationToken);

        image.SetPrimary(true, DateTime.UtcNow, currentUserId);
        await productRepository.UpdateImageAsync(image, cancellationToken);
    }

    private static void EnsureStableSort(ProductQueryDto filter)
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
