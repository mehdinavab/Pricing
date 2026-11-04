using Kahrad.Pricing.Sms.Application.Abstractions.Persistence;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Infrastructure.Background;
using Kahrad.Pricing.Sms.Infrastructure.Options;
using Kahrad.Pricing.Sms.Infrastructure.Persistence;
using Kahrad.Pricing.Sms.Infrastructure.Persistence.Repositories;
using Kahrad.Pricing.Sms.Infrastructure.Services;
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

        var smsSection = configuration.GetSection(SmsIrOptions.SectionName);
        services.Configure<SmsIrOptions>(options =>
        {
            options.ApiKey = smsSection["ApiKey"];
            options.LineNumber = smsSection["LineNumber"];
        });

        services.AddScoped<SmsSieveProcessor>();
        services.TryAddScoped<ISieveProcessor, SmsSieveProcessor>();

        services.AddScoped<ISmsMessageRepository, SmsMessageRepository>();
        services.AddScoped<ISmsProvider, IpeSmsProvider>();
        services.AddScoped<ISmsOutboxProcessor, SmsOutboxProcessor>();

        // Background worker checking for unsent/expired SMS every 2 minutes
        services.AddHostedService<SmsBackgroundWorker>();

        return services;
    }
}
