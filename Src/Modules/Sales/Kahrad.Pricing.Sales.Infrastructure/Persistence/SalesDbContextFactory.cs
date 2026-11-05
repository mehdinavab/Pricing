using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Kahrad.Pricing.Sales.Infrastructure.Persistence;

public sealed class SalesDbContextFactory : IDesignTimeDbContextFactory<SalesDbContext>
{
    public SalesDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SalesDbContext>();
        options.UseSqlServer("Server=localhost;Database=KahradPricing;Trusted_Connection=True;TrustServerCertificate=True;");
        return new SalesDbContext(options.Options);
    }
}
