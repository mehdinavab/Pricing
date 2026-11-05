using Kahrad.Pricing.Sms.Application.Abstractions.Persistence;
using Kahrad.Pricing.Sms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Sms.Infrastructure.Persistence.Repositories;

public sealed class SmsConfigurationRepository(SmsDbContext context) : ISmsConfigurationRepository
{
    public async Task<SmsModuleConfiguration> GetModuleAsync(CancellationToken cancellationToken = default) =>
        await context.ModuleConfigurations.AsNoTracking().SingleOrDefaultAsync(x => x.ModuleKey == "Sms", cancellationToken)
        ?? throw new InvalidOperationException("SMS module configuration is missing; apply SMS migrations.");

    public async Task<SmsProviderConfiguration> GetProviderAsync(long id, CancellationToken cancellationToken = default) =>
        await context.ProviderConfigurations.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new InvalidOperationException("SMS provider configuration was not found.");

    public async Task<IReadOnlyCollection<SmsProviderConfiguration>> GetProvidersAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        var providerIds = ids.Distinct().ToArray();
        return await context.ProviderConfigurations.AsNoTracking().Where(x => providerIds.Contains(x.Id)).ToListAsync(cancellationToken);
    }

    public async Task<long> AddProviderAsync(SmsProviderConfiguration configuration, CancellationToken cancellationToken = default)
    {
        context.ProviderConfigurations.Add(configuration);
        await context.SaveChangesAsync(cancellationToken);
        return configuration.Id;
    }

    public async Task UpdateModuleAsync(SmsModuleConfiguration configuration, CancellationToken cancellationToken = default)
    {
        context.ModuleConfigurations.Update(configuration);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateProviderAsync(SmsProviderConfiguration configuration, CancellationToken cancellationToken = default)
    {
        context.ProviderConfigurations.Update(configuration);
        await context.SaveChangesAsync(cancellationToken);
    }
}
