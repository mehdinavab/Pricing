using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Domain.Entities;
using Kahrad.Pricing.Sms.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kahrad.Pricing.Sms.Infrastructure.Services;

public sealed class SmsProviderResolver(IConfiguration secrets, KavenegarTransport transport,
    ILogger<IpeSmsProvider> logger) : ISmsProviderResolver
{
    public ISmsProvider Resolve(SmsProviderConfiguration configuration)
    {
        if (!configuration.IsEnabled) throw new InvalidOperationException("SMS provider is disabled.");
        var apiKey = secrets[configuration.ApiKeyConfigurationKey];
        return configuration.Provider switch
        {
            "SmsIr" => new IpeSmsProvider(Microsoft.Extensions.Options.Options.Create(new SmsIrOptions
            {
                ApiKey = apiKey, LineNumber = configuration.LineNumber, TimeoutSeconds = configuration.TimeoutSeconds
            }), logger),
            "Kavenegar" => new KavenegarSmsProvider(transport, Microsoft.Extensions.Options.Options.Create(new KavenegarOptions
            {
                ApiKey = apiKey, LineNumber = configuration.LineNumber,
                TimeoutSeconds = configuration.TimeoutSeconds, Templates = configuration.GetTemplateMappings()
            })),
            _ => throw new InvalidOperationException("Unsupported SMS provider configuration.")
        };
    }
}
