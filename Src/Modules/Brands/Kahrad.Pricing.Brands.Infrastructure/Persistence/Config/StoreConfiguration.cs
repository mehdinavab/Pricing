using Kahrad.Pricing.Brands.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kahrad.Pricing.Brands.Infrastructure.Persistence.Config;

public sealed class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("Stores", "brands");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PublicId)
            .IsRequired();

        builder.HasIndex(x => x.PublicId)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.OwnerName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.OwnerPhoneNumber)
            .HasMaxLength(50);

        builder.Property(x => x.OwnerNationalCode)
            .HasMaxLength(20);

        builder.Property(x => x.OwnerEmail)
            .HasMaxLength(200);

        builder.Property(x => x.PictureUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.Address)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.City)
            .HasMaxLength(100);

        builder.Property(x => x.Province)
            .HasMaxLength(100);

        builder.Property(x => x.PostalCode)
            .HasMaxLength(30);

        builder.Property(x => x.Phone)
            .HasMaxLength(50);

        builder.Property(x => x.PriceAdjustmentType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.PriceAdjustmentPercentage)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasIndex(x => new { x.BrandId, x.Code })
            .IsUnique();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
