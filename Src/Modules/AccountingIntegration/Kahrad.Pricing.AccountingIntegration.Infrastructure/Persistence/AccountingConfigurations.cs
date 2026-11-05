using Kahrad.Pricing.AccountingIntegration.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kahrad.Pricing.AccountingIntegration.Infrastructure.Persistence;

internal static class ConfigurationCommon
{
    public static void Audit<TEntity>(EntityTypeBuilder<TEntity> builder) where TEntity : Kahrad.Pricing.BuildingBlocks.Domain.Common.BaseEntity
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.PublicId).IsUnique();
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.Property(x => x.Description).HasMaxLength(500);
    }
}

public sealed class ConnectionConfiguration : IEntityTypeConfiguration<AccountingBrandConnection>
{
    public void Configure(EntityTypeBuilder<AccountingBrandConnection> b)
    {
        b.ToTable("BrandConnections", "accounting"); ConfigurationCommon.Audit(b);
        b.Property(x => x.ProviderCode).HasMaxLength(64).IsRequired();
        b.Property(x => x.BaseUrl).HasMaxLength(500);
        b.Property(x => x.ApiKeySecretReference).HasMaxLength(250);
        b.HasIndex(x => x.BrandId).IsUnique();
    }
}

public sealed class AccountingItemConfiguration : IEntityTypeConfiguration<AccountingItem>
{
    public void Configure(EntityTypeBuilder<AccountingItem> b)
    {
        b.ToTable("Items", "accounting"); ConfigurationCommon.Audit(b);
        b.Property(x => x.ExternalItemId).HasMaxLength(200).IsRequired();
        b.Property(x => x.Name).HasMaxLength(250).IsRequired();
        b.Property(x => x.Code).HasMaxLength(100);
        b.Property(x => x.UnitCode).HasMaxLength(32);
        b.HasIndex(x => new { x.BrandId, x.ExternalItemId }).IsUnique();
        b.HasIndex(x => new { x.BrandId, x.IsPresentInLatestSync });
    }
}

public sealed class AccountingItemPriceConfiguration : IEntityTypeConfiguration<AccountingItemPrice>
{
    public void Configure(EntityTypeBuilder<AccountingItemPrice> b)
    {
        b.ToTable("ItemPrices", "accounting"); ConfigurationCommon.Audit(b);
        b.Property(x => x.Amount).HasPrecision(18, 4).IsRequired();
        b.Property(x => x.CurrencyCode).HasMaxLength(16).IsRequired();
        b.HasIndex(x => new { x.AccountingItemId, x.ObservedAtUtc });
        b.HasOne<AccountingItem>().WithMany().HasForeignKey(x => x.AccountingItemId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<AccountingSyncRun>().WithMany().HasForeignKey(x => x.SourceSyncRunId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class AccountingItemMappingConfiguration : IEntityTypeConfiguration<AccountingItemMapping>
{
    public void Configure(EntityTypeBuilder<AccountingItemMapping> b)
    {
        b.ToTable("ItemMappings", "accounting"); ConfigurationCommon.Audit(b);
        b.Property(x => x.UnitConversionFactor).HasPrecision(18, 8).IsRequired();
        b.HasIndex(x => new { x.BrandId, x.LocalItemId }).IsUnique().HasFilter("[IsActive] = 1");
        b.HasIndex(x => new { x.BrandId, x.AccountingItemId }).IsUnique().HasFilter("[IsActive] = 1");
        b.HasOne<AccountingItem>().WithMany().HasForeignKey(x => x.AccountingItemId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class AccountingRepresentativeConfiguration : IEntityTypeConfiguration<AccountingRepresentative>
{
    public void Configure(EntityTypeBuilder<AccountingRepresentative> b)
    {
        b.ToTable("Representatives", "accounting"); ConfigurationCommon.Audit(b);
        b.Property(x => x.ExternalRepresentativeId).HasMaxLength(200).IsRequired();
        b.Property(x => x.Name).HasMaxLength(250).IsRequired();
        b.Property(x => x.Code).HasMaxLength(100);
        b.HasIndex(x => new { x.BrandId, x.ExternalRepresentativeId }).IsUnique();
    }
}

public sealed class AccountingInvoiceConfiguration : IEntityTypeConfiguration<AccountingInvoice>
{
    public void Configure(EntityTypeBuilder<AccountingInvoice> b)
    {
        b.ToTable("Invoices", "accounting"); ConfigurationCommon.Audit(b);
        b.Property(x => x.ExternalInvoiceId).HasMaxLength(200).IsRequired();
        b.Property(x => x.ExternalRepresentativeId).HasMaxLength(200);
        b.Property(x => x.InvoiceNumber).HasMaxLength(100);
        b.Property(x => x.ExternalStatus).HasMaxLength(100);
        b.HasIndex(x => new { x.BrandId, x.ExternalInvoiceId }).IsUnique();
    }
}

public sealed class AccountingSyncRunConfiguration : IEntityTypeConfiguration<AccountingSyncRun>
{
    public void Configure(EntityTypeBuilder<AccountingSyncRun> b)
    {
        b.ToTable("SyncRuns", "accounting"); ConfigurationCommon.Audit(b);
        b.Property(x => x.ErrorCategory).HasMaxLength(100);
        b.HasIndex(x => new { x.BrandId, x.Type, x.StartedAtUtc });
        b.HasIndex(x => new { x.BrandId, x.Type }).IsUnique().HasFilter("[Status] = 0");
    }
}
