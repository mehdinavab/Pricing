using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Products.Application.DTOs.Features;
using Kahrad.Pricing.Products.Application.DTOs.Gallery;
using Kahrad.Pricing.Products.Application.DTOs.Products;

namespace Kahrad.Pricing.Products.Application.Abstractions.Services;

public interface IProductService
{
    // Product queries
    Task<ProductDto?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<ProductDetailDto?> GetDetailByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<ProductDto?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ProductDto>> GetAllAsync(
        long? groupId = null,
        long? brandId = null,
        bool onlyActive = false,
        CancellationToken cancellationToken = default);

    Task<BaseResponsePaginated<ProductDto>> GetPagedAsync(
        ProductQueryDto filter,
        CancellationToken cancellationToken = default);

    // Product mutations
    Task<long> CreateAsync(
        CreateProductRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        long id,
        UpdateProductRequest request,
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

    Task UpdatePricingFactorsAsync(
        long id,
        UpdateProductPricingFactorsRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    // Pricing estimation
    Task<ProductPriceCalculationDto> CalculateEstimatedPriceAsync(
        long id,
        CancellationToken cancellationToken = default);

    // Features
    Task<IReadOnlyCollection<ProductFeatureDto>> GetFeaturesAsync(
        long productId,
        bool? hasPriceEffect = null,
        CancellationToken cancellationToken = default);

    Task<ProductFeatureDto?> GetFeatureByIdAsync(
        long featureId,
        CancellationToken cancellationToken = default);

    Task<long> AddFeatureAsync(
        long productId,
        AddProductFeatureRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task UpdateFeatureAsync(
        long featureId,
        UpdateProductFeatureRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task DeleteFeatureAsync(
        long featureId,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    // Gallery
    Task<IReadOnlyCollection<ProductImageDto>> GetGalleryAsync(
        long productId,
        CancellationToken cancellationToken = default);

    Task<ProductImageDto?> GetImageByIdAsync(
        long imageId,
        CancellationToken cancellationToken = default);

    Task<long> AddImageAsync(
        long productId,
        AddProductImageRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task UpdateImageAsync(
        long imageId,
        UpdateProductImageRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task DeleteImageAsync(
        long imageId,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task SetPrimaryImageAsync(
        long productId,
        long imageId,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);
}
