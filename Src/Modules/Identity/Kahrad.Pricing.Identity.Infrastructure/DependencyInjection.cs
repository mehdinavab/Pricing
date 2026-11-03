using Kahrad.Pricing.Identity.Application.Abstractions.Persistence;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Entities;
using Kahrad.Pricing.Identity.Infrastructure.Persistence.Repositories;
using Kahrad.Pricing.Identity.Infrastructure.Persistence;
using Kahrad.Pricing.Identity.Infrastructure.Services;
using Kahrad.Pricing.Identity.Infrastructure.Services.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sieve.Models;
using Sieve.Services;

namespace Kahrad.Pricing.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentityDb");
        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Configure ConnectionStrings:IdentityDb or ConnectionStrings:DefaultConnection.");
        }

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 12;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ISieveProcessor, IdentitySieveProcessor>();
        services.Configure<SieveOptions>(options =>
        {
            options.DefaultPageSize = 20;
            options.MaxPageSize = 100;
            options.ThrowExceptions = true;
        });
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }
}