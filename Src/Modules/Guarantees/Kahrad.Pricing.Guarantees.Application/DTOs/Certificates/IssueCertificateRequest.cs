namespace Kahrad.Pricing.Guarantees.Application.DTOs.Certificates;

public sealed class IssueCertificateRequest
{
    public long GuaranteeId { get; init; }
    public long ProductId { get; init; }
    public DateTime PurchaseDate { get; init; }
    public string? CertificateNumber { get; init; }
    public string? ProductSerialNumber { get; init; }
    public long? StoreId { get; init; }
    public string? CustomerName { get; init; }
    public string? CustomerPhoneNumber { get; init; }
    public string? CustomerNationalCode { get; init; }
    public string? Description { get; init; }
}
