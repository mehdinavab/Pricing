using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Kahrad.Pricing.AccountingIntegration.Domain.Entities;
using Kahrad.Pricing.Products.Application.Abstractions.Persistence;

namespace Kahrad.Pricing.AccountingIntegration.Application.Services;

public sealed class AccountingItemMappingService(IAccountingMappingStore store, ILocalItemReader localItems)
{
    public async Task<long> MapAsync(long brandId, long localItemId, long externalItemId,
        decimal unitConversionFactor, Guid? actorId, CancellationToken cancellationToken)
    {
        if (brandId <= 0 || localItemId <= 0 || externalItemId <= 0 || unitConversionFactor <= 0)
            throw new AccountingOperationException("Item mapping identifiers and conversion factor must be positive.");
        var local = await localItems.GetAsync(localItemId, cancellationToken)
            ?? throw new AccountingOperationException("Local item was not found.");
        var external = await store.GetExternalItemAsync(externalItemId, cancellationToken)
            ?? throw new AccountingOperationException("Accounting item was not found.");
        if (external.BrandId != brandId) throw new AccountingOperationException("Accounting item belongs to another brand.");
        if (string.IsNullOrWhiteSpace(external.UnitCode) ||
            !string.Equals(local.UnitCode, external.UnitCode, StringComparison.OrdinalIgnoreCase) || unitConversionFactor != 1)
            throw new AccountingOperationException("Item unit conversion requires an approved mapping rule.");
        var byLocal = await store.GetByLocalItemAsync(brandId, localItemId, cancellationToken);
        if (byLocal is not null)
        {
            if (byLocal.AccountingItemId == externalItemId) return byLocal.Id;
            throw new AccountingOperationException("Local item is already mapped for this brand.");
        }
        if (await store.GetByExternalItemAsync(brandId, externalItemId, cancellationToken) is not null)
            throw new AccountingOperationException("Accounting item is already mapped for this brand.");
        var mapping = new AccountingItemMapping(brandId, localItemId, externalItemId, unitConversionFactor, actorId);
        await store.AddAsync(mapping, cancellationToken);
        return mapping.Id;
    }

    public async Task UnmapAsync(long brandId, long localItemId, CancellationToken cancellationToken)
    {
        var mapping = await store.GetByLocalItemAsync(brandId, localItemId, cancellationToken)
            ?? throw new AccountingOperationException("Active item mapping was not found.");
        await store.DeactivateAsync(mapping, cancellationToken);
    }

    public Task<IReadOnlyList<AccountingItemView>> SearchItemsAsync(long brandId, string? search,
        int page, int pageSize, CancellationToken cancellationToken)
    {
        if (brandId <= 0 || page is < 1 or > 1000000 || pageSize is < 1 or > 100)
            throw new AccountingOperationException("Invalid accounting item pagination.");
        return store.SearchItemsAsync(brandId, search?.Trim(), page, pageSize, cancellationToken);
    }
}
