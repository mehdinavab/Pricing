using Kahrad.Pricing.BuildingBlocks.Domain.Common;

namespace Kahrad.Pricing.AccountingIntegration.Domain.Entities;

/// <summary>External document mirror. Its business meaning is intentionally unresolved.</summary>
public sealed class AccountingInvoice : BaseEntity
{
    private AccountingInvoice() { }
    public AccountingInvoice(long brandId, string externalInvoiceId, DateTime seenAtUtc)
    {
        if (brandId <= 0) throw new ArgumentOutOfRangeException(nameof(brandId));
        ArgumentException.ThrowIfNullOrWhiteSpace(externalInvoiceId);
        BrandId = brandId; ExternalInvoiceId = externalInvoiceId.Trim(); LastSeenAtUtc = seenAtUtc;
    }

    public long BrandId { get; private set; }
    public string ExternalInvoiceId { get; private set; } = null!;
    public string? ExternalRepresentativeId { get; private set; }
    public string? InvoiceNumber { get; private set; }
    public string? ExternalStatus { get; private set; }
    public DateTime? IssuedAtUtc { get; private set; }
    public DateTime LastSeenAtUtc { get; private set; }

    public void Update(string? externalRepresentativeId, string? invoiceNumber, string? externalStatus,
        DateTime? issuedAtUtc, DateTime seenAtUtc)
    {
        if (externalRepresentativeId?.Length > 200 || invoiceNumber?.Length > 100 || externalStatus?.Length > 100)
            throw new ArgumentException("Accounting invoice metadata is too long.");
        ExternalRepresentativeId = externalRepresentativeId?.Trim();
        InvoiceNumber = invoiceNumber?.Trim(); ExternalStatus = externalStatus?.Trim();
        IssuedAtUtc = issuedAtUtc; LastSeenAtUtc = seenAtUtc;
    }
}
