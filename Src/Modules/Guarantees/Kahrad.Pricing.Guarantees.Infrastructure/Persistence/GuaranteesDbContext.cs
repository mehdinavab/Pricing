using Kahrad.Pricing.Guarantees.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Guarantees.Infrastructure.Persistence;

public sealed class GuaranteesDbContext : DbContext
{
    public GuaranteesDbContext(DbContextOptions<GuaranteesDbContext> options) : base(options)
    {
    }

    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<Guarantee> Guarantees => Set<Guarantee>();
    public DbSet<GuaranteeCertificate> GuaranteeCertificates => Set<GuaranteeCertificate>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("guarantees");

        builder.ApplyConfigurationsFromAssembly(typeof(GuaranteesDbContext).Assembly);
    }
}
