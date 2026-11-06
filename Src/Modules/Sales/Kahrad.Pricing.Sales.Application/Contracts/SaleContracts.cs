using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;
using Kahrad.Pricing.Sales.Domain.Entities;

namespace Kahrad.Pricing.Sales.Application.Contracts;

public sealed record PublishedSalePrice(long BrandId, long ProductId, long RepresentativeId,
    decimal ProductionCost, decimal RepresentativePrice, decimal CustomerSellingPrice,
    string CurrencyCode, long Version);

public interface IPublishedSalePriceReader
{
    Task<PublishedSalePrice?> GetAsync(long productId, long representativeId, CancellationToken cancellationToken);
}

public interface ISaleFinalizationStore
{
    Task<SoldProductUnit?> GetByOperationIdAsync(Guid operationId, CancellationToken cancellationToken);
    Task<SoldProductUnit> SaveAtomicallyAsync(SoldProductUnit sale, AccountingSalePostOutbox outbox, CancellationToken cancellationToken);
}

public sealed class PublishedSalePricesNotConfiguredException()
    : KahradPricingException(
        "Published sale prices are not configured.",
        true,
        ErrorCodes.ServiceUnavailable);
