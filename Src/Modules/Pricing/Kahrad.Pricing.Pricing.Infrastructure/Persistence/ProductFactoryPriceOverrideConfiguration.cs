using Kahrad.Pricing.Pricing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kahrad.Pricing.Pricing.Infrastructure.Persistence;

public sealed class ProductFactoryPriceOverrideConfiguration : IEntityTypeConfiguration<ProductFactoryPriceOverride>
{
    public void Configure(EntityTypeBuilder<ProductFactoryPriceOverride> b)
    {
        b.ToTable("ProductFactoryPriceOverrides", "pricing");
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.PublicId).IsUnique();
        b.HasIndex(x => new { x.BrandId, x.ProductId }).IsUnique();
        b.Property(x => x.FixedFactorySellingPrice).HasPrecision(18, 4);
        b.Property(x => x.ManualFactoryPriceAdjustment).HasPrecision(18, 4);
        b.Property(x => x.RowVersion).IsRowVersion();
    }
}
