using Kahrad.Pricing.Guarantees.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kahrad.Pricing.Guarantees.Infrastructure.Persistence.Config;

public sealed class GuaranteeCertificateConfiguration : IEntityTypeConfiguration<GuaranteeCertificate>
{
    public void Configure(EntityTypeBuilder<GuaranteeCertificate> builder)
    {
        builder.ToTable("GuaranteeCertificates", "guarantees");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PublicId)
            .IsRequired();

        builder.HasIndex(x => x.PublicId)
            .IsUnique();

        builder.Property(x => x.CertificateNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.CertificateNumber)
            .IsUnique();

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.HasIndex(x => x.ProductId);

        builder.Property(x => x.ProductSerialNumber)
            .HasMaxLength(100);

        builder.HasIndex(x => x.ProductSerialNumber);

        builder.Property(x => x.PurchaseDate)
            .IsRequired();

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.ExpiryDate)
            .IsRequired();

        builder.Property(x => x.CustomerName)
            .HasMaxLength(200);

        builder.Property(x => x.CustomerPhoneNumber)
            .HasMaxLength(50);

        builder.Property(x => x.CustomerNationalCode)
            .HasMaxLength(20);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.RevocationReason)
            .HasMaxLength(500);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
