using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Kahrad.Pricing.Products.Infrastructure.Persistence;

public sealed class ProductsDbContextFactory : IDesignTimeDbContextFactory<ProductsDbContext>
{
    public ProductsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProductsDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=KahradPricing;Trusted_Connection=True;TrustServerCertificate=True;");

        return new ProductsDbContext(optionsBuilder.Options);
    }
}
