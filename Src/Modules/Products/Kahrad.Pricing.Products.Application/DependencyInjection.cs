using FluentValidation;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kahrad.Pricing.Products.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddProductsApplication(
        this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assembly);

        services.AddAutoMapper(cfg => { }, assembly);

        services.AddScoped<IProductGroupService, ProductGroupService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<ItemCatalogService>();
        services.AddSingleton<ILanguageResolver, LanguageResolver>();

        return services;
    }
}
