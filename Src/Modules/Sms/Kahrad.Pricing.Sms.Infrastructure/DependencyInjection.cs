using Kahrad.Pricing.Sms.Application.Abstractions.Persistence;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Infrastructure.Persistence;
using Kahrad.Pricing.Sms.Infrastructure.Persistence.Repositories;
using Kahrad.Pricing.Sms.Infrastructure.Services;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sieve.Models;
using Sieve.Services;

namespace Kahrad.Pricing.Sms.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSmsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SmsDb");
        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Configure ConnectionStrings:SmsDb or ConnectionStrings:DefaultConnection.");
        }

        services.AddDbContext<SmsDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.Configure<SieveOptions>(options =>
        {
            options.DefaultPageSize = 20;
            options.MaxPageSize = 100;
            options.ThrowExceptions = true;
        });

        services.AddScoped<SmsSieveProcessor>();
        services.TryAddScoped<ISieveProcessor, SmsSieveProcessor>();

        services.AddScoped<ISmsMessageRepository, SmsMessageRepository>();
        services.AddScoped<ISmsConfigurationRepository, SmsConfigurationRepository>();
        services.AddSingleton(_ => new KavenegarTransport(new HttpClient(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(5)
        }) { Timeout = Timeout.InfiniteTimeSpan }));
        services.AddScoped<ISmsProviderResolver, SmsProviderResolver>();
        services.AddScoped<IOtpNotificationSender, IdentityOtpNotificationSender>();


        return services;
    }
}
