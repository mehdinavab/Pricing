namespace Kahrad.Pricing.AccountingIntegration.Application.Contracts;

public sealed record AccountingInvoiceView(long Id, long BrandId, string ExternalInvoiceId,
    string? ExternalRepresentativeId, string? InvoiceNumber, string? ExternalStatus,
    DateTime? IssuedAtUtc, DateTime LastSeenAtUtc);

public interface IAccountingInvoiceReader
{
    Task<IReadOnlyList<AccountingInvoiceView>> SearchAsync(long brandId, string? search,
        int page, int pageSize, CancellationToken cancellationToken);
}
