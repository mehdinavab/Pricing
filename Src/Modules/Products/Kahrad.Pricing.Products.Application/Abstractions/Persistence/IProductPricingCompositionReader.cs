namespace Kahrad.Pricing.Products.Application.Abstractions.Persistence;

public sealed record ProductPricingComponent(long ItemId, decimal QuantityUsed, string UnitCode);

public sealed record ProductPricingComposition(long ProductId, long? BrandId,
    IReadOnlyList<ProductPricingComponent> Components);

public interface IProductPricingCompositionReader
{
    Task<ProductPricingComposition?> GetAsync(long productId, CancellationToken cancellationToken);
}
