using Kahrad.Pricing.Sales.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Sales.Infrastructure.Persistence;

public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : DbContext(options)
{
    public DbSet<SoldProductUnit> SoldProductUnits => Set<SoldProductUnit>();
    public DbSet<AccountingSalePostOutbox> AccountingSalePosts => Set<AccountingSalePostOutbox>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("sales");
        builder.ApplyConfigurationsFromAssembly(typeof(SalesDbContext).Assembly);
    }
}
