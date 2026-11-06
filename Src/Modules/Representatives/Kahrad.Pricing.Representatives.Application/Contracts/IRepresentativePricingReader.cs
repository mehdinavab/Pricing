namespace Kahrad.Pricing.Representatives.Application.Contracts;

public sealed record RepresentativePricingIdentity(long Id, long BrandId, bool IsActive);

public interface IRepresentativePricingReader
{
    Task<RepresentativePricingIdentity?> GetAsync(long representativeId,
        CancellationToken cancellationToken);
}
