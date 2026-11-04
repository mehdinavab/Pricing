using FluentValidation;
using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Brands.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kahrad.Pricing.Brands.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddBrandsApplication(
        this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assembly);

        services.AddAutoMapper(cfg => { }, assembly);

        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<IStoreService, StoreService>();

        return services;
    }
}
