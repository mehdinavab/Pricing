using Kahrad.Pricing.Pricing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kahrad.Pricing.Pricing.Infrastructure.Persistence;

public sealed class BrandPricingPolicyConfiguration : IEntityTypeConfiguration<BrandPricingPolicy>
{
    public void Configure(EntityTypeBuilder<BrandPricingPolicy> b)
    {
        ConfigureBase(b);
        b.ToTable("BrandPricingPolicies", "pricing");
        b.HasIndex(x => x.BrandId).IsUnique();
        b.Property(x => x.BeautyPercentage).HasPrecision(18, 6);
        b.Property(x => x.FactorySalesPercentage).HasPrecision(18, 6);
        b.Property(x => x.CommercePercentage).HasPrecision(18, 6);
        b.Property(x => x.RepresentativeFactor).HasPrecision(18, 8);
        b.Property(x => x.FactoryRoundingStep).HasPrecision(18, 4);
    }

    internal static void ConfigureBase<TEntity>(EntityTypeBuilder<TEntity> b)
        where TEntity : Kahrad.Pricing.BuildingBlocks.Domain.Common.BaseEntity
    {
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.PublicId).IsUnique();
        b.Property(x => x.RowVersion).IsRowVersion();
        b.Property(x => x.Description).HasMaxLength(500);
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class RepresentativePricingPolicyConfiguration
    : IEntityTypeConfiguration<RepresentativePricingPolicy>
{
    public void Configure(EntityTypeBuilder<RepresentativePricingPolicy> b)
    {
        BrandPricingPolicyConfiguration.ConfigureBase(b);
        b.ToTable("RepresentativePricingPolicies", "pricing");
        b.HasIndex(x => new { x.BrandId, x.RepresentativeId }).IsUnique();
        b.Property(x => x.SalesPercentage).HasPrecision(18, 6);
        b.Property(x => x.GeneralDiscountPercentage).HasPrecision(18, 6);
        b.Property(x => x.CustomerRoundingStep).HasPrecision(18, 4);
    }
}

public sealed class RepresentativeProductDiscountConfiguration
    : IEntityTypeConfiguration<RepresentativeProductDiscount>
{
    public void Configure(EntityTypeBuilder<RepresentativeProductDiscount> b)
    {
        BrandPricingPolicyConfiguration.ConfigureBase(b);
        b.ToTable("RepresentativeProductDiscounts", "pricing");
        b.HasIndex(x => new { x.BrandId, x.RepresentativeId, x.ProductId }).IsUnique();
        b.HasIndex(x => new { x.BrandId, x.ProductId });
        b.Property(x => x.DiscountPercentage).HasPrecision(18, 6);
    }
}
