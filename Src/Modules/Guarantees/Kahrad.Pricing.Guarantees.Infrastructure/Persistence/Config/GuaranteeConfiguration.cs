using Kahrad.Pricing.Guarantees.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kahrad.Pricing.Guarantees.Infrastructure.Persistence.Config;

public sealed class GuaranteeConfiguration : IEntityTypeConfiguration<Guarantee>
{
    public void Configure(EntityTypeBuilder<Guarantee> builder)
    {
        builder.ToTable("Guarantees", "guarantees");

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

        builder.Property(x => x.DurationMonths)
            .IsRequired();

        builder.Property(x => x.DurationDays)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.TermsAndConditions)
            .HasMaxLength(4000);

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasIndex(x => new { x.VendorId, x.Code })
            .IsUnique();

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasMany(x => x.Certificates)
            .WithOne(x => x.Guarantee)
            .HasForeignKey(x => x.GuaranteeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(x => x.Certificates)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
