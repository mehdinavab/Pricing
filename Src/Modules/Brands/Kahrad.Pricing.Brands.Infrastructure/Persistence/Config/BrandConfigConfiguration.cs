using Kahrad.Pricing.Brands.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kahrad.Pricing.Brands.Infrastructure.Persistence.Config;

public sealed class BrandConfigConfiguration : IEntityTypeConfiguration<BrandConfig>
{
    public void Configure(EntityTypeBuilder<BrandConfig> builder)
    {
        builder.ToTable("BrandConfigs", "brands");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PublicId)
            .IsRequired();

        builder.HasIndex(x => x.PublicId)
            .IsUnique();

        builder.Property(x => x.Key)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.DisplayName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.DataType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Category)
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasIndex(x => new { x.BrandId, x.Key })
            .IsUnique();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
