using Kahrad.Pricing.Products.Domain.Entities;

namespace Kahrad.Pricing.Products.Application.Abstractions.Persistence;

public interface IProductRepository
{
    // Product queries
    Task<Product?> GetByIdAsync(
        long id,
        bool includeGroup = false,
        bool includeFeatures = false,
        bool includeGallery = false,
        CancellationToken cancellationToken = default);

    Task<Product?> GetByPublicIdAsync(
        Guid publicId,
        bool includeGroup = false,
        bool includeFeatures = false,
        bool includeGallery = false,
        CancellationToken cancellationToken = default);

    Task<Product?> GetByCodeAsync(
        string code,
        bool includeGroup = false,
        bool includeFeatures = false,
        bool includeGallery = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Product>> GetAllAsync(
        long? groupId = null,
        long? brandId = null,
        bool onlyActive = false,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string code,
        long? excludeProductId = null,
        CancellationToken cancellationToken = default);

    // Product mutations
    Task<long> AddAsync(
        Product product,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Product product,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Product product,
        CancellationToken cancellationToken = default);

    IQueryable<Product> GetQueryable();

    // Feature operations
    Task<ProductFeature?> GetFeatureByIdAsync(
        long featureId,
        CancellationToken cancellationToken = default);

    Task<ProductFeature?> GetFeatureByPublicIdAsync(
        Guid publicId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ProductFeature>> GetFeaturesByProductIdAsync(
        long productId,
        bool? hasPriceEffect = null,
        CancellationToken cancellationToken = default);

    Task<long> AddFeatureAsync(
        ProductFeature feature,
        CancellationToken cancellationToken = default);

    Task UpdateFeatureAsync(
        ProductFeature feature,
        CancellationToken cancellationToken = default);

    Task DeleteFeatureAsync(
        ProductFeature feature,
        CancellationToken cancellationToken = default);

    // Gallery operations
    Task<ProductImage?> GetImageByIdAsync(
        long imageId,
        CancellationToken cancellationToken = default);

    Task<ProductImage?> GetImageByPublicIdAsync(
        Guid publicId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ProductImage>> GetGalleryByProductIdAsync(
        long productId,
        CancellationToken cancellationToken = default);

    Task<long> AddImageAsync(
        ProductImage image,
        CancellationToken cancellationToken = default);

    Task UpdateImageAsync(
        ProductImage image,
        CancellationToken cancellationToken = default);

    Task DeleteImageAsync(
        ProductImage image,
        CancellationToken cancellationToken = default);

    Task ClearPrimaryImagesAsync(
        long productId,
        long? excludeImageId = null,
        CancellationToken cancellationToken = default);
}
