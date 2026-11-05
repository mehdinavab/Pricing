using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Kahrad.Pricing.AccountingIntegration.Infrastructure.Persistence;

public sealed class AccountingDbContextFactory : IDesignTimeDbContextFactory<AccountingDbContext>
{
    public AccountingDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AccountingDbContext>();
        builder.UseSqlServer("Server=localhost;Database=KahradPricing;Trusted_Connection=True;TrustServerCertificate=True;");
        return new AccountingDbContext(builder.Options);
    }
}
