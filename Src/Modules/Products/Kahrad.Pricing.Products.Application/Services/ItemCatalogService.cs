using Kahrad.Pricing.Products.Application.Abstractions.Persistence;
using Kahrad.Pricing.Products.Application.Exceptions;

namespace Kahrad.Pricing.Products.Application.Services;

public sealed class ItemCatalogService(IItemCatalogStore store)
{
    public Task<long> CreateAsync(string code, string name, string unitCode, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(unitCode) ||
            code.Trim().Length > 100 || name.Trim().Length > 250 || unitCode.Trim().Length > 32)
            throw new ProductOperationException("Local item details are invalid.");
        return store.CreateItemAsync(code.Trim(), name.Trim(), unitCode.Trim(), ct);
    }

    public Task<IReadOnlyList<LocalItemView>> SearchAsync(string? search, int page, int pageSize, CancellationToken ct)
    {
        if (page is < 1 or > 1000000 || pageSize is < 1 or > 100)
            throw new ProductOperationException("Invalid local item pagination.");
        return store.SearchItemsAsync(search?.Trim(), page, pageSize, ct);
    }

    public Task<IReadOnlyList<ProductItemView>> GetCompositionAsync(long productId, CancellationToken ct) =>
        store.GetCompositionAsync(productId, ct);

    public Task ReplaceCompositionAsync(long productId, IReadOnlyList<ProductItemInput> items, CancellationToken ct)
    {
        if (productId <= 0 || items is null || items.Count is < 1 or > 500 ||
            items.Any(x => x.ItemId <= 0 || x.QuantityUsed <= 0 || decimal.Round(x.QuantityUsed, 4) != x.QuantityUsed) ||
            items.Select(x => x.ItemId).Distinct().Count() != items.Count)
            throw new ProductOperationException("Product composition requires distinct items with valid quantities.");
        return store.ReplaceCompositionAsync(productId, items, ct);
    }
}
