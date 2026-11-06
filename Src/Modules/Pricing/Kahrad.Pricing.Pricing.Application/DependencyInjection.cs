using Kahrad.Pricing.Pricing.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kahrad.Pricing.Pricing.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPricingApplication(this IServiceCollection services)
    {
        services.AddScoped<ProductPriceOverrideService>();
        services.AddScoped<PricingPolicyService>();
        services.AddScoped<ProductPricePreviewService>();
        return services;
    }
}
