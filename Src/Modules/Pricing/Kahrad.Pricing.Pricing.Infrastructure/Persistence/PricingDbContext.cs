using Kahrad.Pricing.Pricing.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Pricing.Infrastructure.Persistence;

public sealed class PricingDbContext(DbContextOptions<PricingDbContext> options) : DbContext(options)
{
    public DbSet<ProductFactoryPriceOverride> ProductFactoryPriceOverrides => Set<ProductFactoryPriceOverride>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("pricing");
        builder.ApplyConfigurationsFromAssembly(typeof(PricingDbContext).Assembly);
    }
}
