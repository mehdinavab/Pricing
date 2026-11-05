using Kahrad.Pricing.Sms.Domain.Entities;

namespace Kahrad.Pricing.Sms.Application.Abstractions.Persistence;

public interface ISmsConfigurationRepository
{
    Task<SmsModuleConfiguration> GetModuleAsync(CancellationToken cancellationToken = default);
    Task<SmsProviderConfiguration> GetProviderAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SmsProviderConfiguration>> GetProvidersAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
    Task<long> AddProviderAsync(SmsProviderConfiguration configuration, CancellationToken cancellationToken = default);
    Task UpdateModuleAsync(SmsModuleConfiguration configuration, CancellationToken cancellationToken = default);
    Task UpdateProviderAsync(SmsProviderConfiguration configuration, CancellationToken cancellationToken = default);
}
