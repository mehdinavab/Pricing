using FluentValidation;
using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kahrad.Pricing.Guarantees.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddGuaranteesApplication(
        this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assembly);

        services.AddAutoMapper(cfg => { }, assembly);

        services.AddScoped<IVendorService, VendorService>();
        services.AddScoped<IGuaranteeService, GuaranteeService>();
        services.AddScoped<IGuaranteeCertificateService, GuaranteeCertificateService>();

        return services;
    }
}
