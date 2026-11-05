using Kahrad.Pricing.AccountingIntegration.Domain.Entities;

namespace Kahrad.Pricing.AccountingIntegration.Application.Contracts;

public sealed record AccountingConnectionView(long BrandId, string ProviderCode, string? BaseUrl,
    bool HasSecretReference, bool IsEnabled);

public interface IAccountingConnectionStore
{
    Task<IReadOnlyList<AccountingConnectionView>> ListAsync(CancellationToken cancellationToken);
    Task<AccountingBrandConnection?> GetByBrandAsync(long brandId, CancellationToken cancellationToken);
    Task SaveAsync(AccountingBrandConnection connection, CancellationToken cancellationToken);
}
