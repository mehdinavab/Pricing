using Kahrad.Pricing.Representatives.Domain.Entities;

namespace Kahrad.Pricing.Representatives.Application.Contracts;

public interface IRepresentativeStore
{
    Task<Representative?> GetAsync(long id, CancellationToken cancellationToken);
    Task<Representative?> GetByCodeAsync(long brandId, string code, CancellationToken cancellationToken);
    Task<RepresentativeAccountingMapping?> GetMappingByExternalAsync(long brandId, long externalId, CancellationToken cancellationToken);
    Task<RepresentativeAccountingMapping?> GetMappingByLocalAsync(long brandId, long representativeId, CancellationToken cancellationToken);
    Task<long> ImportAsync(Representative representative, long accountingRepresentativeId, Guid? actorId, CancellationToken cancellationToken);
    Task AddMappingAsync(RepresentativeAccountingMapping mapping, CancellationToken cancellationToken);
}
