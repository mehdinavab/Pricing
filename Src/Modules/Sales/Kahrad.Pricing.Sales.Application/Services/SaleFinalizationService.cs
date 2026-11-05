using Kahrad.Pricing.Sales.Application.Contracts;
using Kahrad.Pricing.Sales.Domain.Entities;

namespace Kahrad.Pricing.Sales.Application.Services;

public sealed class SaleFinalizationService(IPublishedSalePriceReader prices, ISaleFinalizationStore store)
{
    public async Task<SoldProductUnit> FinalizeAsync(Guid operationId, long productId,
        long representativeId, CancellationToken cancellationToken)
    {
        if (operationId == Guid.Empty || productId <= 0 || representativeId <= 0)
            throw new ArgumentException("Sale operation, product and representative are required.");
        var existing = await store.GetByOperationIdAsync(operationId, cancellationToken);
        if (existing is not null)
        {
            if (existing.ProductId != productId || existing.RepresentativeId != representativeId)
                throw new InvalidOperationException("Sale operation ID was already used for another sale.");
            return existing;
        }
        var published = await prices.GetAsync(productId, representativeId, cancellationToken)
            ?? throw new PublishedSalePricesNotConfiguredException();
        var sale = new SoldProductUnit(operationId, published.BrandId, productId, representativeId,
            published.ProductionCost, published.RepresentativePrice, published.CustomerSellingPrice,
            published.CurrencyCode, published.Version, DateTime.UtcNow);
        return await store.SaveAtomicallyAsync(sale, new AccountingSalePostOutbox(sale), cancellationToken);
    }
}
