using Kahrad.Pricing.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kahrad.Pricing.Identity.Infrastructure.Persistence.Config
{
    public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs", "identity");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.PublicId)
                .IsUnique();

            builder.Property(x => x.Action)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.EntityType)
                .HasMaxLength(150);

            builder.Property(x => x.IpAddress)
                .HasMaxLength(45);

            builder.Property(x => x.CorrelationId)
                .HasMaxLength(100);

            builder.HasIndex(x => x.CreatedAtUtc);

            builder.HasIndex(x => new { x.EntityType, x.EntityPublicId });

            builder.Property(x => x.BeforeData)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.AfterData)
                .HasColumnType("nvarchar(max)");
        }
    }
}
