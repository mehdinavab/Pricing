using Kahrad.Pricing.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kahrad.Pricing.Identity.Infrastructure.Persistence.Config
{
    public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions", "identity");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PublicId)
                .IsRequired();

            builder.HasIndex(x => x.PublicId)
                .IsUnique();

            builder.Property(x => x.Code)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();
        }
    }
}
