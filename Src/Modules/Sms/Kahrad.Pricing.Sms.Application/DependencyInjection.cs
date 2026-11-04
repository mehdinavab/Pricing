using FluentValidation;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Kahrad.Pricing.Sms.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSmsApplication(
        this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assembly);

        services.AddAutoMapper(cfg => { }, assembly);

        services.AddScoped<ISmsService, SmsService>();

        return services;
    }
}
