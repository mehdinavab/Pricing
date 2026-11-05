namespace Kahrad.Pricing.Products.Application.Abstractions.Persistence;

public interface IProductBrandReader
{
    Task<long?> GetBrandIdAsync(long productId, CancellationToken cancellationToken);
}
