using Kahrad.Pricing.Representatives.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Representatives.Infrastructure.Persistence;

public sealed class RepresentativesDbContext(DbContextOptions<RepresentativesDbContext> options) : DbContext(options)
{
    public DbSet<Representative> Representatives => Set<Representative>();
    public DbSet<RepresentativeAccountingMapping> AccountingMappings => Set<RepresentativeAccountingMapping>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("representatives");
        builder.ApplyConfigurationsFromAssembly(typeof(RepresentativesDbContext).Assembly);
    }
}
