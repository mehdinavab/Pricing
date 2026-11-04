using Kahrad.Pricing.Guarantees.Application.Abstractions.Persistence;
using Kahrad.Pricing.Guarantees.Infrastructure.Persistence;
using Kahrad.Pricing.Guarantees.Infrastructure.Persistence.Repositories;
using Kahrad.Pricing.Guarantees.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sieve.Models;
using Sieve.Services;

namespace Kahrad.Pricing.Guarantees.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddGuaranteesInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("GuaranteesDb");
        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Configure ConnectionStrings:GuaranteesDb or ConnectionStrings:DefaultConnection.");
        }

        services.AddDbContext<GuaranteesDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.Configure<SieveOptions>(options =>
        {
            options.DefaultPageSize = 20;
            options.MaxPageSize = 100;
            options.ThrowExceptions = true;
        });

        services.AddScoped<GuaranteesSieveProcessor>();
        services.TryAddScoped<ISieveProcessor, GuaranteesSieveProcessor>();

        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<IGuaranteeRepository, GuaranteeRepository>();
        services.AddScoped<IGuaranteeCertificateRepository, GuaranteeCertificateRepository>();

        return services;
    }
}
