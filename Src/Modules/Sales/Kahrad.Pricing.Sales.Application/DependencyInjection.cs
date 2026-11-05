using Kahrad.Pricing.Sales.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kahrad.Pricing.Sales.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesApplication(this IServiceCollection services)
    {
        services.AddScoped<SaleFinalizationService>();
        return services;
    }
}
