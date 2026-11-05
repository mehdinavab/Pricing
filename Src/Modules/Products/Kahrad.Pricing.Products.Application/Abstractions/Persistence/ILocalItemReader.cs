namespace Kahrad.Pricing.Products.Application.Abstractions.Persistence;

public sealed record LocalItemReference(long Id, string Code, string UnitCode);

public interface ILocalItemReader
{
    Task<LocalItemReference?> GetAsync(long itemId, CancellationToken cancellationToken);
}
