using Kahrad.Pricing.AccountingIntegration.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kahrad.Pricing.AccountingIntegration.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountingApplication(this IServiceCollection services)
    {
        services.AddScoped<AccountingSyncService>();
        services.AddScoped<AccountingItemMappingService>();
        services.AddScoped<AccountingConnectionService>();
        return services;
    }
}
