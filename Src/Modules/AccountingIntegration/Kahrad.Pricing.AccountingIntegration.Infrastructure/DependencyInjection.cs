using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Kahrad.Pricing.AccountingIntegration.Infrastructure.Persistence;
using Kahrad.Pricing.AccountingIntegration.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kahrad.Pricing.AccountingIntegration.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("AccountingDb")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Configure the accounting SQL Server connection string.");
        services.AddDbContext<AccountingDbContext>(options => options.UseSqlServer(connection));
        services.AddScoped<IAccountingMirrorStore, AccountingMirrorStore>();
        services.AddScoped<IAccountingMappingStore, AccountingMappingStore>();
        services.AddScoped<IAccountingRepresentativeReader, AccountingRepresentativeReader>();
        services.AddScoped<IAccountingInvoiceReader, AccountingInvoiceReader>();
        services.AddScoped<IAccountingSyncRunReader, AccountingSyncRunReader>();
        services.AddScoped<IAccountingConnectionStore, AccountingConnectionStore>();
        services.AddScoped<IEffectiveItemPriceReader, EffectiveItemPriceReader>();
        services.AddScoped<IAccountingProviderClient, NotConfiguredAccountingProviderClient>();
        return services;
    }
}
