using Kahrad.Pricing.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Products.Infrastructure.Persistence;

public sealed class ProductsDbContext : DbContext
{
    public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options)
    {
    }

    public DbSet<ProductGroup> ProductGroups => Set<ProductGroup>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductFeature> ProductFeatures => Set<ProductFeature>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<ProductItem> ProductItems => Set<ProductItem>();

    public DbSet<Language> Languages => Set<Language>();
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<Specification> Specifications => Set<Specification>();
    public DbSet<ProductTranslation> ProductTranslations => Set<ProductTranslation>();
    public DbSet<FeatureTranslation> FeatureTranslations => Set<FeatureTranslation>();
    public DbSet<SpecificationTranslation> SpecificationTranslations => Set<SpecificationTranslation>();
    public DbSet<ProductSpecification> ProductSpecifications => Set<ProductSpecification>();
    public DbSet<ProductSpecificationTranslation> ProductSpecificationTranslations => Set<ProductSpecificationTranslation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("products");

        builder.ApplyConfigurationsFromAssembly(typeof(ProductsDbContext).Assembly);
    }
}
