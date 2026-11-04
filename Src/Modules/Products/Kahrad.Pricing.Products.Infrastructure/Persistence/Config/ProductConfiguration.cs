using Kahrad.Pricing.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kahrad.Pricing.Products.Infrastructure.Persistence.Config;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", "products");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PublicId)
            .IsRequired();

        builder.HasIndex(x => x.PublicId)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.Code)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.GroupId)
            .IsRequired();

        builder.HasIndex(x => x.GroupId);

        builder.Property(x => x.BrandId);

        builder.HasIndex(x => x.BrandId);

        builder.Property(x => x.BasePrice)
            .HasPrecision(18, 4)
            .HasDefaultValue(0);

        builder.Property(x => x.BeautyFactorPercentage)
            .HasPrecision(18, 4)
            .HasDefaultValue(0);

        builder.Property(x => x.CommercialFactorPercentage)
            .HasPrecision(18, 4)
            .HasDefaultValue(0);

        builder.Property(x => x.SalesFactorPercentage)
            .HasPrecision(18, 4)
            .HasDefaultValue(0);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.Group)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Features)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Gallery)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Features)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(x => x.Gallery)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
