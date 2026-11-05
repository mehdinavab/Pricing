using Kahrad.Pricing.Representatives.Application.Contracts;
using Kahrad.Pricing.Representatives.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kahrad.Pricing.Representatives.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRepresentativesInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("RepresentativesDb")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Configure the representatives SQL Server connection string.");
        services.AddDbContext<RepresentativesDbContext>(options => options.UseSqlServer(connection));
        services.AddScoped<IRepresentativeStore, RepresentativeStore>();
        return services;
    }
}
