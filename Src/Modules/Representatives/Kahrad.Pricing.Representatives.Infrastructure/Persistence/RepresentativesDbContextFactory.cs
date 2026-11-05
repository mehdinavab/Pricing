using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Kahrad.Pricing.Representatives.Infrastructure.Persistence;

public sealed class RepresentativesDbContextFactory : IDesignTimeDbContextFactory<RepresentativesDbContext>
{
    public RepresentativesDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<RepresentativesDbContext>();
        options.UseSqlServer("Server=localhost;Database=KahradPricing;Trusted_Connection=True;TrustServerCertificate=True;");
        return new RepresentativesDbContext(options.Options);
    }
}
