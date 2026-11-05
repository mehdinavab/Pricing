using Kahrad.Pricing.Sales.Application.Contracts;

namespace Kahrad.Pricing.Sales.Infrastructure.Services;

public sealed class NotConfiguredPublishedSalePriceReader : IPublishedSalePriceReader
{
    public Task<PublishedSalePrice?> GetAsync(long productId, long representativeId, CancellationToken cancellationToken)
        => throw new PublishedSalePricesNotConfiguredException();
}
