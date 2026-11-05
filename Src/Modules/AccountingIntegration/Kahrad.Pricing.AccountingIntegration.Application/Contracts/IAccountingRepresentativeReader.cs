namespace Kahrad.Pricing.AccountingIntegration.Application.Contracts;

public sealed record ExternalRepresentativeReference(long Id, long BrandId, string ExternalRepresentativeId,
    string Name, string? Code, bool IsPresentInLatestSync);

public interface IAccountingRepresentativeReader
{
    Task<ExternalRepresentativeReference?> GetAsync(long id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ExternalRepresentativeReference>> SearchAsync(long brandId, string? search,
        int page, int pageSize, CancellationToken cancellationToken);
}
