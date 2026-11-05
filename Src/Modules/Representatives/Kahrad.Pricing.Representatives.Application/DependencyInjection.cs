using Kahrad.Pricing.Representatives.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kahrad.Pricing.Representatives.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddRepresentativesApplication(this IServiceCollection services)
    {
        services.AddScoped<RepresentativeImportService>();
        return services;
    }
}
