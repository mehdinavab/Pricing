using Kahrad.Pricing.Brands.Application.Abstractions.Persistence;
using Kahrad.Pricing.Brands.Infrastructure.Persistence;
using Kahrad.Pricing.Brands.Infrastructure.Persistence.Repositories;
using Kahrad.Pricing.Brands.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sieve.Models;
using Sieve.Services;

namespace Kahrad.Pricing.Brands.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBrandsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BrandsDb");
        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Configure ConnectionStrings:BrandsDb or ConnectionStrings:DefaultConnection.");
        }

        services.AddDbContext<BrandsDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.Configure<SieveOptions>(options =>
        {
            options.DefaultPageSize = 20;
            options.MaxPageSize = 100;
            options.ThrowExceptions = true;
        });

        services.AddScoped<BrandsSieveProcessor>();
        services.TryAddScoped<ISieveProcessor, BrandsSieveProcessor>();

        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IBrandConfigRepository, BrandConfigRepository>();
        services.AddScoped<IStoreRepository, StoreRepository>();

        return services;
    }
}
