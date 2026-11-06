using Kahrad.Pricing.Products.Application.Abstractions.Persistence;
using Kahrad.Pricing.Products.Infrastructure.Persistence;
using Kahrad.Pricing.Products.Infrastructure.Persistence.Repositories;
using Kahrad.Pricing.Products.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sieve.Models;
using Sieve.Services;

namespace Kahrad.Pricing.Products.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddProductsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ProductsDb");
        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Configure ConnectionStrings:ProductsDb or ConnectionStrings:DefaultConnection.");
        }

        services.AddDbContext<ProductsDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.Configure<SieveOptions>(options =>
        {
            options.DefaultPageSize = 20;
            options.MaxPageSize = 100;
            options.ThrowExceptions = true;
        });

        services.AddScoped<ProductsSieveProcessor>();
        services.TryAddScoped<ISieveProcessor, ProductsSieveProcessor>();

        services.AddScoped<IProductGroupRepository, ProductGroupRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICatalogRepository, CatalogRepository>();
        services.AddScoped<ILocalItemReader, LocalItemReader>();
        services.AddScoped<IProductBrandReader, ProductBrandReader>();
        services.AddScoped<IProductPricingCompositionReader, ProductPricingCompositionReader>();
        services.AddScoped<IItemCatalogStore, ItemCatalogStore>();

        return services;
    }
}
