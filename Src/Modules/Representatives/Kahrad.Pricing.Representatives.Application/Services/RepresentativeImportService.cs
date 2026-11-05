using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Kahrad.Pricing.Representatives.Application.Contracts;
using Kahrad.Pricing.Representatives.Domain.Entities;

namespace Kahrad.Pricing.Representatives.Application.Services;

public sealed class RepresentativeImportService(IAccountingRepresentativeReader external, IRepresentativeStore local)
{
    public async Task<long> ImportAsync(long brandId, long accountingRepresentativeId, string localCode,
        Guid? actorId, CancellationToken cancellationToken)
    {
        var record = await RequireExternalAsync(brandId, accountingRepresentativeId, cancellationToken);
        if (string.IsNullOrWhiteSpace(localCode) || localCode.Trim().Length > 100)
            throw new RepresentativeOperationException("A valid representative code is required.");
        var existing = await local.GetMappingByExternalAsync(brandId, accountingRepresentativeId, cancellationToken);
        if (existing is not null) return existing.RepresentativeId;
        if (await local.GetByCodeAsync(brandId, localCode.Trim().ToUpperInvariant(), cancellationToken) is not null)
            throw new RepresentativeOperationException("Representative code already exists; match the existing representative explicitly.");
        var representative = new Representative(brandId, localCode, record.Name);
        return await local.ImportAsync(representative, accountingRepresentativeId, actorId, cancellationToken);
    }

    public async Task<long> MatchAsync(long brandId, long representativeId, long accountingRepresentativeId,
        Guid? actorId, CancellationToken cancellationToken)
    {
        await RequireExternalAsync(brandId, accountingRepresentativeId, cancellationToken);
        var representative = await local.GetAsync(representativeId, cancellationToken)
            ?? throw new RepresentativeOperationException("Local representative was not found.");
        if (representative.BrandId != brandId)
            throw new RepresentativeOperationException("Representative belongs to another brand.");
        var externalMapping = await local.GetMappingByExternalAsync(brandId, accountingRepresentativeId, cancellationToken);
        if (externalMapping is not null)
        {
            if (externalMapping.RepresentativeId == representativeId) return externalMapping.Id;
            throw new RepresentativeOperationException("Accounting representative is already matched.");
        }
        if (await local.GetMappingByLocalAsync(brandId, representativeId, cancellationToken) is not null)
            throw new RepresentativeOperationException("Local representative is already matched.");
        var mapping = new RepresentativeAccountingMapping(brandId, representativeId, accountingRepresentativeId, actorId);
        await local.AddMappingAsync(mapping, cancellationToken);
        return mapping.Id;
    }

    private async Task<ExternalRepresentativeReference> RequireExternalAsync(long brandId, long externalId, CancellationToken ct)
    {
        if (brandId <= 0 || externalId <= 0) throw new RepresentativeOperationException("Representative identifiers must be positive.");
        var record = await external.GetAsync(externalId, ct)
            ?? throw new RepresentativeOperationException("Accounting representative was not found.");
        if (record.BrandId != brandId) throw new RepresentativeOperationException("Accounting representative belongs to another brand.");
        if (!record.IsPresentInLatestSync) throw new RepresentativeOperationException("Accounting representative was absent from the latest complete sync.");
        return record;
    }
}
