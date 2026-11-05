using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Kahrad.Pricing.Pricing.Infrastructure.Persistence;

public sealed class PricingDbContextFactory : IDesignTimeDbContextFactory<PricingDbContext>
{
    public PricingDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PricingDbContext>();
        options.UseSqlServer("Server=localhost;Database=KahradPricing;Trusted_Connection=True;TrustServerCertificate=True;");
        return new PricingDbContext(options.Options);
    }
}
