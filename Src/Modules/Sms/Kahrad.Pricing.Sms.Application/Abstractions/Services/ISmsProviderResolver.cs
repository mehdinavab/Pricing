using Kahrad.Pricing.Sms.Domain.Entities;

namespace Kahrad.Pricing.Sms.Application.Abstractions.Services;

public interface ISmsProviderResolver
{
    ISmsProvider Resolve(SmsProviderConfiguration configuration);
}
