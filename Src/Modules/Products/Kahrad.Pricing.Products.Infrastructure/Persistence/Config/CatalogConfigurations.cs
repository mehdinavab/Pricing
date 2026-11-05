using Kahrad.Pricing.Products.Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kahrad.Pricing.Products.Infrastructure.Persistence.Config;

public sealed class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> b)
    {
        b.ToTable("Languages", "products", t => t.HasCheckConstraint("CK_Language_DefaultActive", "[IsDefault] = 0 OR ([IsActive] = 1 AND [IsDeleted] = 0)"));
        b.HasKey(x => x.Id); b.HasIndex(x => x.PublicId).IsUnique();
        b.Property(x => x.Code).HasMaxLength(16).UseCollation("Latin1_General_100_CI_AS").IsRequired();
        b.Property(x => x.Name).HasMaxLength(100).IsRequired();
        b.HasIndex(x => x.Code).IsUnique();
        b.HasIndex(x => x.IsDefault).IsUnique().HasFilter("[IsDefault] = 1 AND [IsDeleted] = 0");
        b.Property(x => x.RowVersion).IsRowVersion(); b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> b)
    {
        b.ToTable("Features", "products"); b.HasKey(x => x.Id);
        b.HasIndex(x => x.PublicId).IsUnique(); b.HasIndex(x => x.Code).IsUnique();
        b.Property(x => x.Code).HasMaxLength(100).IsRequired(); b.Property(x => x.Name).HasMaxLength(150).IsRequired();

        b.Property(x => x.RowVersion).IsRowVersion(); b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class SpecificationConfiguration : IEntityTypeConfiguration<Specification>
{
    public void Configure(EntityTypeBuilder<Specification> b)
    {
        b.ToTable("Specifications", "products"); b.HasKey(x => x.Id);
        b.HasIndex(x => x.PublicId).IsUnique(); b.HasIndex(x => x.Code).IsUnique();
        b.Property(x => x.Code).HasMaxLength(100).IsRequired(); b.Property(x => x.Name).HasMaxLength(150).IsRequired();
        b.Property(x => x.Unit).HasMaxLength(50);
        b.Property(x => x.RowVersion).IsRowVersion(); b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class ProductTranslationConfiguration : IEntityTypeConfiguration<ProductTranslation>
{
    public void Configure(EntityTypeBuilder<ProductTranslation> b)
    {
        b.ToTable("ProductTranslations", "products"); b.HasKey(x => x.Id); b.HasIndex(x => x.PublicId).IsUnique();
        b.HasIndex(x => new { x.ProductId, x.LanguageId }).IsUnique();
        b.Property(x => x.Name).HasMaxLength(250).IsRequired();
        b.Property(x => x.LocalizedDescription).HasMaxLength(1000);
        b.HasOne<Language>().WithMany().HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Product>().WithMany(x => x.Translations).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        b.Property(x => x.RowVersion).IsRowVersion(); b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class FeatureTranslationConfiguration : IEntityTypeConfiguration<FeatureTranslation>
{
    public void Configure(EntityTypeBuilder<FeatureTranslation> b)
    {
        b.ToTable("FeatureTranslations", "products"); b.HasKey(x => x.Id); b.HasIndex(x => x.PublicId).IsUnique();
        b.HasIndex(x => new { x.FeatureId, x.LanguageId }).IsUnique();
        b.Property(x => x.Name).HasMaxLength(150).IsRequired();

        b.HasOne<Language>().WithMany().HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Feature>().WithMany().HasForeignKey(x => x.FeatureId).OnDelete(DeleteBehavior.Cascade);
        b.Property(x => x.RowVersion).IsRowVersion(); b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class SpecificationTranslationConfiguration : IEntityTypeConfiguration<SpecificationTranslation>
{
    public void Configure(EntityTypeBuilder<SpecificationTranslation> b)
    {
        b.ToTable("SpecificationTranslations", "products"); b.HasKey(x => x.Id); b.HasIndex(x => x.PublicId).IsUnique();
        b.HasIndex(x => new { x.SpecificationId, x.LanguageId }).IsUnique();
        b.Property(x => x.Name).HasMaxLength(150).IsRequired();

        b.HasOne<Language>().WithMany().HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Specification>().WithMany().HasForeignKey(x => x.SpecificationId).OnDelete(DeleteBehavior.Cascade);
        b.Property(x => x.RowVersion).IsRowVersion(); b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class ProductSpecificationTranslationConfiguration : IEntityTypeConfiguration<ProductSpecificationTranslation>
{
    public void Configure(EntityTypeBuilder<ProductSpecificationTranslation> b)
    {
        b.ToTable("ProductSpecificationTranslations", "products"); b.HasKey(x => x.Id); b.HasIndex(x => x.PublicId).IsUnique();
        b.HasIndex(x => new { x.ProductSpecificationId, x.LanguageId }).IsUnique();
        b.Property(x => x.Value).HasMaxLength(500).IsRequired();

        b.HasOne<Language>().WithMany().HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<ProductSpecification>().WithMany().HasForeignKey(x => x.ProductSpecificationId).OnDelete(DeleteBehavior.Cascade);
        b.Property(x => x.RowVersion).IsRowVersion(); b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class ProductSpecificationConfiguration : IEntityTypeConfiguration<ProductSpecification>
{
    public void Configure(EntityTypeBuilder<ProductSpecification> b)
    {
        b.ToTable("ProductSpecifications", "products", t => t.HasCheckConstraint("CK_ProductSpecification_OneValue",
            "(CASE WHEN [TextValue] IS NULL THEN 0 ELSE 1 END + CASE WHEN [NumberValue] IS NULL THEN 0 ELSE 1 END + CASE WHEN [BooleanValue] IS NULL THEN 0 ELSE 1 END + CASE WHEN [DateValue] IS NULL THEN 0 ELSE 1 END + CASE WHEN [CodeValue] IS NULL THEN 0 ELSE 1 END) = 1"));
        b.HasKey(x => x.Id); b.HasIndex(x => x.PublicId).IsUnique();
        b.HasIndex(x => new { x.ProductId, x.SpecificationId }).IsUnique();
        b.Property(x => x.TextValue).HasMaxLength(500); b.Property(x => x.CodeValue).HasMaxLength(100);
        b.Property(x => x.NumberValue).HasPrecision(18, 4); b.Property(x => x.DateValue).HasColumnType("date");
        b.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne<Specification>().WithMany().HasForeignKey(x => x.SpecificationId).OnDelete(DeleteBehavior.Restrict);
        b.Property(x => x.RowVersion).IsRowVersion(); b.HasQueryFilter(x => !x.IsDeleted);
    }
}
