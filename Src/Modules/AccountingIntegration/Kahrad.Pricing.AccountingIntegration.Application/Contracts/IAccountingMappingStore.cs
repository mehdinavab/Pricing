using Kahrad.Pricing.AccountingIntegration.Domain.Entities;

namespace Kahrad.Pricing.AccountingIntegration.Application.Contracts;

public sealed record ExternalItemReference(long Id, long BrandId, string ExternalItemId, string? UnitCode);
public sealed record AccountingItemView(long Id, string ExternalItemId, string Name, string? Code,
    string? UnitCode, bool IsPresentInLatestSync, decimal? LatestPrice, string? CurrencyCode, long? LocalItemId);

public interface IAccountingMappingStore
{
    Task<ExternalItemReference?> GetExternalItemAsync(long accountingItemId, CancellationToken cancellationToken);
    Task<AccountingItemMapping?> GetByLocalItemAsync(long brandId, long localItemId, CancellationToken cancellationToken);
    Task<AccountingItemMapping?> GetByExternalItemAsync(long brandId, long accountingItemId, CancellationToken cancellationToken);
    Task AddAsync(AccountingItemMapping mapping, CancellationToken cancellationToken);
    Task DeactivateAsync(AccountingItemMapping mapping, CancellationToken cancellationToken);
    Task<IReadOnlyList<AccountingItemView>> SearchItemsAsync(long brandId, string? search, int page, int pageSize, CancellationToken cancellationToken);
}
