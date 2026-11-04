using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Kahrad.Pricing.Guarantees.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kahrad.Pricing.Guarantees.Domain.Entities;

[Table("GuaranteeCertificates", Schema = "guarantees")]
[Index(nameof(PublicId), IsUnique = true)]
[Index(nameof(CertificateNumber), IsUnique = true)]
[Index(nameof(GuaranteeId))]
[Index(nameof(ProductId))]
[Index(nameof(ProductSerialNumber))]
[Index(nameof(Status))]
[Index(nameof(IsDeleted))]
public sealed class GuaranteeCertificate : BaseEntity
{
    public long GuaranteeId { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string CertificateNumber { get; private set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public long ProductId { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string? ProductSerialNumber { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public long? StoreId { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime PurchaseDate { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime StartDate { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public DateTime ExpiryDate { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string? CustomerName { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string? CustomerPhoneNumber { get; private set; }

    public string? CustomerNationalCode { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public GuaranteeCertificateStatus Status { get; private set; } = GuaranteeCertificateStatus.Active;

    public string? RevocationReason { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    public Guarantee Guarantee { get; private set; } = null!;

    private GuaranteeCertificate()
    {
    }

    public GuaranteeCertificate(
        long guaranteeId,
        string certificateNumber,
        long productId,
        DateTime purchaseDate,
        DateTime expiryDate,
        DateTime? startDate = null,
        string? productSerialNumber = null,
        long? storeId = null,
        string? customerName = null,
        string? customerPhoneNumber = null,
        string? customerNationalCode = null,
        string? description = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(certificateNumber);

        if (guaranteeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(guaranteeId), "Guarantee ID must be positive.");

        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be positive.");

        if (expiryDate <= purchaseDate)
            throw new ArgumentException("Expiry date must be after purchase date.", nameof(expiryDate));

        GuaranteeId = guaranteeId;
        CertificateNumber = certificateNumber.Trim().ToUpperInvariant();
        ProductId = productId;
        PurchaseDate = purchaseDate;
        StartDate = startDate ?? purchaseDate;
        ExpiryDate = expiryDate;
        ProductSerialNumber = productSerialNumber?.Trim();
        StoreId = storeId;
        CustomerName = customerName?.Trim();
        CustomerPhoneNumber = customerPhoneNumber?.Trim();
        CustomerNationalCode = customerNationalCode?.Trim();
        Description = description?.Trim();
        Status = GuaranteeCertificateStatus.Active;
    }

    public void Revoke(string reason, DateTime utcNow, Guid? userId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        Status = GuaranteeCertificateStatus.Revoked;
        RevocationReason = reason.Trim();
        RevokedAtUtc = utcNow;
        SetUpdated(userId, utcNow);
    }

    public void Extend(int additionalMonths, DateTime utcNow, Guid? userId = null)
    {
        if (additionalMonths <= 0)
            throw new ArgumentOutOfRangeException(nameof(additionalMonths), "Additional months must be positive.");

        ExpiryDate = ExpiryDate.AddMonths(additionalMonths);
        if (Status == GuaranteeCertificateStatus.Expired && ExpiryDate > utcNow)
        {
            Status = GuaranteeCertificateStatus.Active;
        }

        SetUpdated(userId, utcNow);
    }

    public void MarkClaimed(DateTime utcNow, Guid? userId = null)
    {
        Status = GuaranteeCertificateStatus.Claimed;
        SetUpdated(userId, utcNow);
    }

    public void CheckExpiration(DateTime utcNow)
    {
        if (Status == GuaranteeCertificateStatus.Active && utcNow > ExpiryDate)
        {
            Status = GuaranteeCertificateStatus.Expired;
        }
    }

    public void UpdateCustomerInfo(
        string? customerName,
        string? customerPhoneNumber,
        string? customerNationalCode,
        string? description,
        DateTime utcNow,
        Guid? userId = null)
    {
        CustomerName = customerName?.Trim();
        CustomerPhoneNumber = customerPhoneNumber?.Trim();
        CustomerNationalCode = customerNationalCode?.Trim();
        Description = description?.Trim();

        SetUpdated(userId, utcNow);
    }
}
