using Kahrad.Pricing.Sales.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kahrad.Pricing.Sales.Infrastructure.Persistence;

public sealed class SoldProductUnitConfiguration : IEntityTypeConfiguration<SoldProductUnit>
{
    public void Configure(EntityTypeBuilder<SoldProductUnit> b)
    {
        b.ToTable("SoldProductUnits", "sales");
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.PublicId).IsUnique();
        b.HasIndex(x => x.OperationId).IsUnique();
        b.HasIndex(x => new { x.BrandId, x.ProductId, x.FinalizedAtUtc });
        b.Property(x => x.RowVersion).IsRowVersion();
        b.Property(x => x.ProductionCostSnapshot).HasPrecision(18, 4);
        b.Property(x => x.RepresentativePriceSnapshot).HasPrecision(18, 4);
        b.Property(x => x.CustomerSellingPriceSnapshot).HasPrecision(18, 4);
        b.Property(x => x.CurrencyCode).HasMaxLength(16).IsRequired();
    }
}

public sealed class AccountingSalePostOutboxConfiguration : IEntityTypeConfiguration<AccountingSalePostOutbox>
{
    public void Configure(EntityTypeBuilder<AccountingSalePostOutbox> b)
    {
        b.ToTable("AccountingSalePostOutbox", "sales");
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.PublicId).IsUnique();
        b.HasIndex(x => x.IdempotencyKey).IsUnique();
        b.HasIndex(x => new { x.Status, x.NextAttemptAtUtc });
        b.Property(x => x.IdempotencyKey).HasMaxLength(100).IsRequired();
        b.Property(x => x.ExternalReference).HasMaxLength(200);
        b.Property(x => x.ErrorCategory).HasMaxLength(100);
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasOne<SoldProductUnit>().WithOne()
            .HasForeignKey<AccountingSalePostOutbox>(x => x.SoldProductUnitPublicId)
            .HasPrincipalKey<SoldProductUnit>(x => x.PublicId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
