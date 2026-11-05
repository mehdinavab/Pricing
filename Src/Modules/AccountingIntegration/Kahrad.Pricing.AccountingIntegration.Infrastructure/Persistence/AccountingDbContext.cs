using Kahrad.Pricing.AccountingIntegration.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.AccountingIntegration.Infrastructure.Persistence;

public sealed class AccountingDbContext(DbContextOptions<AccountingDbContext> options) : DbContext(options)
{
    public DbSet<AccountingBrandConnection> Connections => Set<AccountingBrandConnection>();
    public DbSet<AccountingItem> Items => Set<AccountingItem>();
    public DbSet<AccountingItemPrice> ItemPrices => Set<AccountingItemPrice>();
    public DbSet<AccountingItemMapping> ItemMappings => Set<AccountingItemMapping>();
    public DbSet<AccountingRepresentative> Representatives => Set<AccountingRepresentative>();
    public DbSet<AccountingInvoice> Invoices => Set<AccountingInvoice>();
    public DbSet<AccountingSyncRun> SyncRuns => Set<AccountingSyncRun>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("accounting");
        builder.ApplyConfigurationsFromAssembly(typeof(AccountingDbContext).Assembly);
    }
}
