using Kahrad.Pricing.Products.Application.Abstractions.Persistence;
using Kahrad.Pricing.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Kahrad.Pricing.Products.Application.Exceptions;

namespace Kahrad.Pricing.Products.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository(ProductsDbContext context) : IProductRepository
{
    // ==========================================
    // Product Queries
    // ==========================================

    public async Task<Product?> GetByIdAsync(
        long id,
        bool includeGroup = false,
        bool includeFeatures = false,
        bool includeGallery = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Products.AsQueryable();

        if (includeGroup)
            query = query.Include(x => x.Group);

        if (includeFeatures)
            query = query.Include(x => x.Features.OrderBy(f => f.DisplayOrder));

        if (includeGallery)
            query = query.Include(x => x.Gallery.OrderBy(g => g.DisplayOrder));

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Product?> GetByPublicIdAsync(
        Guid publicId,
        bool includeGroup = false,
        bool includeFeatures = false,
        bool includeGallery = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Products.AsQueryable();

        if (includeGroup)
            query = query.Include(x => x.Group);

        if (includeFeatures)
            query = query.Include(x => x.Features.OrderBy(f => f.DisplayOrder));

        if (includeGallery)
            query = query.Include(x => x.Gallery.OrderBy(g => g.DisplayOrder));

        return await query.FirstOrDefaultAsync(x => x.PublicId == publicId, cancellationToken);
    }

    public async Task<Product?> GetByCodeAsync(
        string code,
        bool includeGroup = false,
        bool includeFeatures = false,
        bool includeGallery = false,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        var query = context.Products.AsQueryable();

        if (includeGroup)
            query = query.Include(x => x.Group);

        if (includeFeatures)
            query = query.Include(x => x.Features.OrderBy(f => f.DisplayOrder));

        if (includeGallery)
            query = query.Include(x => x.Gallery.OrderBy(g => g.DisplayOrder));

        return await query.FirstOrDefaultAsync(x => x.Code == normalizedCode, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Product>> GetAllAsync(
        long? groupId = null,
        long? brandId = null,
        bool onlyActive = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Products
            .AsNoTracking()
            .Include(x => x.Group)
            .AsQueryable();

        if (groupId.HasValue)
            query = query.Where(x => x.GroupId == groupId.Value);

        if (brandId.HasValue)
            query = query.Where(x => x.BrandId == brandId.Value);

        if (onlyActive)
            query = query.Where(x => x.IsActive);

        return await query
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await context.Products
            .AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        long? excludeProductId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return await context.Products
            .AnyAsync(
                x => x.Code == normalizedCode &&
                     (!excludeProductId.HasValue || x.Id != excludeProductId.Value),
                cancellationToken);
    }

    // ==========================================
    // Product Mutations
    // ==========================================

    public async Task<long> AddAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        await context.Products.AddAsync(product, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await SyncDefaultContentAsync(product, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return product.Id;
    }

    public async Task UpdateAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateContentAsync(Product product, CancellationToken cancellationToken = default)
    {
        await SyncDefaultContentAsync(product, cancellationToken);
        context.Products.Update(product);
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task SyncDefaultContentAsync(Product product, CancellationToken ct)
    {
        var language = await context.Languages.SingleOrDefaultAsync(x => x.IsActive && x.IsDefault, ct)
            ?? throw new ProductOperationException("Catalog default language is missing.");
        var translation = await context.ProductTranslations.SingleOrDefaultAsync(x => x.ProductId == product.Id && x.LanguageId == language.Id, ct);
        if (translation is null)
        {
            translation = new ProductTranslation(product.Id, language.Id, product.Name, product.Description);
            translation.SetCreated(product.UpdatedById ?? product.CreatedById, DateTime.UtcNow);
            context.ProductTranslations.Add(translation);
        }
        else { translation.Update(product.Name, product.Description); translation.SetUpdated(product.UpdatedById, DateTime.UtcNow); }
    }

    public async Task DeleteAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync(cancellationToken);
    }

    public IQueryable<Product> GetQueryable()
    {
        return context.Products.AsQueryable();
    }

    // ==========================================
    // Feature Operations
    // ==========================================

    public async Task<ProductFeature?> GetFeatureByIdAsync(
        long featureId,
        CancellationToken cancellationToken = default)
    {
        return await context.ProductFeatures
            .FirstOrDefaultAsync(x => x.Id == featureId, cancellationToken);
    }

    public async Task<ProductFeature?> GetFeatureByPublicIdAsync(
        Guid publicId,
        CancellationToken cancellationToken = default)
    {
        return await context.ProductFeatures
            .FirstOrDefaultAsync(x => x.PublicId == publicId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ProductFeature>> GetFeaturesByProductIdAsync(
        long productId,
        bool? hasPriceEffect = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.ProductFeatures
            .AsNoTracking()
            .Where(x => x.ProductId == productId);

        if (hasPriceEffect.HasValue)
            query = query.Where(x => x.HasPriceEffect == hasPriceEffect.Value);

        return await query
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> AddFeatureAsync(
        ProductFeature feature,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        if (!feature.FeatureId.HasValue)
        {
            var definition = new Feature($"FEATURE-{Guid.NewGuid():N}", feature.Name);
            definition.SetCreated(feature.CreatedById, DateTime.UtcNow);
            context.Features.Add(definition);
            await context.SaveChangesAsync(cancellationToken);
            var fallback = await context.Languages.SingleAsync(x => x.IsActive && x.IsDefault, cancellationToken);
            var label = new FeatureTranslation(definition.Id, fallback.Id, definition.Name);
            label.SetCreated(feature.CreatedById, DateTime.UtcNow); context.FeatureTranslations.Add(label);
            feature.AssignDefinition(definition.Id);
        }
        await context.ProductFeatures.AddAsync(feature, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return feature.Id;
    }

    public async Task UpdateFeatureAsync(
        ProductFeature feature,
        CancellationToken cancellationToken = default)
    {
        if (feature.FeatureId.HasValue)
        {
            var definition = await context.Features.SingleAsync(x => x.Id == feature.FeatureId.Value, cancellationToken);
            var fallback = await context.Languages.SingleAsync(x => x.IsActive && x.IsDefault, cancellationToken);
            var translation = await context.FeatureTranslations.SingleOrDefaultAsync(x => x.FeatureId == definition.Id && x.LanguageId == fallback.Id, cancellationToken);
            definition.SetDefaultName(feature.Name); definition.SetUpdated(feature.UpdatedById, DateTime.UtcNow);
            if (translation is null) context.FeatureTranslations.Add(new FeatureTranslation(definition.Id, fallback.Id, feature.Name));
            else { translation.Update(feature.Name); translation.SetUpdated(feature.UpdatedById, DateTime.UtcNow); }
        }
        context.ProductFeatures.Update(feature);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteFeatureAsync(
        ProductFeature feature,
        CancellationToken cancellationToken = default)
    {
        context.ProductFeatures.Update(feature);
        await context.SaveChangesAsync(cancellationToken);
    }

    // ==========================================
    // Gallery Operations
    // ==========================================

    public async Task<ProductImage?> GetImageByIdAsync(
        long imageId,
        CancellationToken cancellationToken = default)
    {
        return await context.ProductImages
            .FirstOrDefaultAsync(x => x.Id == imageId, cancellationToken);
    }

    public async Task<ProductImage?> GetImageByPublicIdAsync(
        Guid publicId,
        CancellationToken cancellationToken = default)
    {
        return await context.ProductImages
            .FirstOrDefaultAsync(x => x.PublicId == publicId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ProductImage>> GetGalleryByProductIdAsync(
        long productId,
        CancellationToken cancellationToken = default)
    {
        return await context.ProductImages
            .AsNoTracking()
            .Where(x => x.ProductId == productId)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> AddImageAsync(
        ProductImage image,
        CancellationToken cancellationToken = default)
    {
        await context.ProductImages.AddAsync(image, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return image.Id;
    }

    public async Task UpdateImageAsync(
        ProductImage image,
        CancellationToken cancellationToken = default)
    {
        context.ProductImages.Update(image);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteImageAsync(
        ProductImage image,
        CancellationToken cancellationToken = default)
    {
        context.ProductImages.Update(image);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task ClearPrimaryImagesAsync(
        long productId,
        long? excludeImageId = null,
        CancellationToken cancellationToken = default)
    {
        var primaryImages = await context.ProductImages
            .Where(x => x.ProductId == productId && x.IsPrimary && (!excludeImageId.HasValue || x.Id != excludeImageId.Value))
            .ToListAsync(cancellationToken);

        if (primaryImages.Count != 0)
        {
            var utcNow = DateTime.UtcNow;
            foreach (var img in primaryImages)
            {
                img.SetPrimary(false, utcNow);
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
