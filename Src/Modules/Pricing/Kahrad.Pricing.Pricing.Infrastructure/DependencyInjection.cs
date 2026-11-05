using Kahrad.Pricing.Pricing.Application.Contracts;
using Kahrad.Pricing.Pricing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kahrad.Pricing.Pricing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPricingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("PricingDb")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Configure the pricing SQL Server connection string.");
        services.AddDbContext<PricingDbContext>(options => options.UseSqlServer(connection));
        services.AddScoped<IPriceOverrideStore, PriceOverrideStore>();
        return services;
    }
}
