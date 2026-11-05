namespace Kahrad.Pricing.Products.Application.Abstractions.Persistence;

public sealed record LocalItemView(long Id, string Code, string Name, string UnitCode);
public sealed record ProductItemView(long ItemId, string ItemCode, string UnitCode, decimal QuantityUsed);
public sealed record ProductItemInput(long ItemId, decimal QuantityUsed);

public interface IItemCatalogStore
{
    Task<long> CreateItemAsync(string code, string name, string unitCode, CancellationToken cancellationToken);
    Task<IReadOnlyList<LocalItemView>> SearchItemsAsync(string? search, int page, int pageSize, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductItemView>> GetCompositionAsync(long productId, CancellationToken cancellationToken);
    Task ReplaceCompositionAsync(long productId, IReadOnlyList<ProductItemInput> items, CancellationToken cancellationToken);
}
