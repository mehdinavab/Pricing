using Kahrad.Pricing.Sales.Application.Contracts;
using Kahrad.Pricing.Sales.Infrastructure.Persistence;
using Kahrad.Pricing.Sales.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kahrad.Pricing.Sales.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSalesInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("SalesDb")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Configure the sales SQL Server connection string.");
        services.AddDbContext<SalesDbContext>(options => options.UseSqlServer(connection));
        services.AddScoped<ISaleFinalizationStore, SaleFinalizationStore>();
        services.AddScoped<IPublishedSalePriceReader, NotConfiguredPublishedSalePriceReader>();
        return services;
    }
}
