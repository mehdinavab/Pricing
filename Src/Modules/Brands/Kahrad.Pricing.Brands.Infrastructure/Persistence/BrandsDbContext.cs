using Kahrad.Pricing.Brands.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Brands.Infrastructure.Persistence;

public sealed class BrandsDbContext : DbContext
{
    public BrandsDbContext(DbContextOptions<BrandsDbContext> options) : base(options)
    {
    }

    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<BrandConfig> BrandConfigs => Set<BrandConfig>();
    public DbSet<Store> Stores => Set<Store>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("brands");

        builder.ApplyConfigurationsFromAssembly(typeof(BrandsDbContext).Assembly);
    }
}
