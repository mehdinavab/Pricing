using Kahrad.Pricing.Sms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Sms.Infrastructure.Persistence;

public sealed class SmsDbContext : DbContext
{
    public SmsDbContext(DbContextOptions<SmsDbContext> options) : base(options)
    {
    }

    public DbSet<SmsMessage> Messages => Set<SmsMessage>();

    public DbSet<SmsLog> Logs => Set<SmsLog>();
    public DbSet<SmsProviderConfiguration> ProviderConfigurations => Set<SmsProviderConfiguration>();
    public DbSet<SmsModuleConfiguration> ModuleConfigurations => Set<SmsModuleConfiguration>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("sms");

        builder.ApplyConfigurationsFromAssembly(typeof(SmsDbContext).Assembly);
    }
}
