using Kahrad.Pricing.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kahrad.Pricing.Identity.Infrastructure.Persistence.Config
{
    public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens", "identity");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.PublicId)
                .IsUnique();

            builder.Property(x => x.TokenHash)
                .HasMaxLength(500)
                .IsRequired();

            builder.HasIndex(x => x.TokenHash)
                .IsUnique();

            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => x.FamilyId);

            builder.Property(x => x.CreatedByIp)
                .HasMaxLength(45);

            builder.Property(x => x.RevokedByIp)
                .HasMaxLength(45);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<RefreshToken>()
                .WithMany()
                .HasForeignKey(x => x.ReplacedByTokenId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();
        }
    }
}