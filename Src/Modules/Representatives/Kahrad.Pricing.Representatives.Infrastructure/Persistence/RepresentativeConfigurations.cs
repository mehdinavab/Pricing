using Kahrad.Pricing.Representatives.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kahrad.Pricing.Representatives.Infrastructure.Persistence;

public sealed class RepresentativeConfiguration : IEntityTypeConfiguration<Representative>
{
    public void Configure(EntityTypeBuilder<Representative> b)
    {
        b.ToTable("Representatives", "representatives");
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.PublicId).IsUnique();
        b.HasIndex(x => new { x.BrandId, x.Code }).IsUnique().HasFilter("[IsDeleted] = 0");
        b.Property(x => x.Code).HasMaxLength(100).IsRequired();
        b.Property(x => x.Name).HasMaxLength(250).IsRequired();
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class RepresentativeAccountingMappingConfiguration : IEntityTypeConfiguration<RepresentativeAccountingMapping>
{
    public void Configure(EntityTypeBuilder<RepresentativeAccountingMapping> b)
    {
        b.ToTable("AccountingMappings", "representatives");
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.PublicId).IsUnique();
        b.HasIndex(x => new { x.BrandId, x.RepresentativeId }).IsUnique().HasFilter("[IsActive] = 1");
        b.HasIndex(x => new { x.BrandId, x.AccountingRepresentativeId }).IsUnique().HasFilter("[IsActive] = 1");
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasOne<Representative>().WithMany().HasForeignKey(x => x.RepresentativeId).OnDelete(DeleteBehavior.Restrict);
    }
}
