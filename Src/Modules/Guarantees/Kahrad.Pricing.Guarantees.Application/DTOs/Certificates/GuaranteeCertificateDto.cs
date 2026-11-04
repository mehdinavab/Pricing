using Kahrad.Pricing.Guarantees.Domain.Enums;

namespace Kahrad.Pricing.Guarantees.Application.DTOs.Certificates;

public class GuaranteeCertificateDto
{
    public long Id { get; init; }
    public Guid PublicId { get; init; }
    public long GuaranteeId { get; init; }
    public string? GuaranteeName { get; init; }
    public string? VendorName { get; init; }
    public string CertificateNumber { get; init; } = null!;
    public long ProductId { get; init; }
    public string? ProductSerialNumber { get; init; }
    public long? StoreId { get; init; }
    public DateTime PurchaseDate { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime ExpiryDate { get; init; }
    public string? CustomerName { get; init; }
    public string? CustomerPhoneNumber { get; init; }
    public string? CustomerNationalCode { get; init; }
    public GuaranteeCertificateStatus Status { get; init; }
    public string? RevocationReason { get; init; }
    public DateTime? RevokedAtUtc { get; init; }
    public string? Description { get; init; }
    public bool IsExpired => DateTime.UtcNow > ExpiryDate;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
