using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kahrad.Pricing.Guarantees.Domain.Entities;

[Table("Guarantees", Schema = "guarantees")]
[Index(nameof(PublicId), IsUnique = true)]
[Index(nameof(VendorId), nameof(Code), IsUnique = true)]
[Index(nameof(VendorId))]
[Index(nameof(IsDeleted))]
public sealed class Guarantee : BaseEntity
{
    private readonly List<GuaranteeCertificate> _certificates = [];

    public long VendorId { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string Name { get; private set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public string Code { get; private set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public int DurationMonths { get; private set; }

    public int DurationDays { get; private set; }

    public string? TermsAndConditions { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public bool IsActive { get; private set; } = true;

    public Vendor Vendor { get; private set; } = null!;

    public IReadOnlyCollection<GuaranteeCertificate> Certificates => _certificates.AsReadOnly();

    private Guarantee()
    {
    }

    public Guarantee(
        long vendorId,
        string name,
        string code,
        int durationMonths,
        string? description = null,
        int durationDays = 0,
        string? termsAndConditions = null,
        bool isActive = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        if (vendorId <= 0)
            throw new ArgumentOutOfRangeException(nameof(vendorId), "Vendor ID must be positive.");

        if (durationMonths < 0)
            throw new ArgumentOutOfRangeException(nameof(durationMonths), "Duration in months cannot be negative.");

        if (durationDays < 0)
            throw new ArgumentOutOfRangeException(nameof(durationDays), "Duration in days cannot be negative.");

        if (durationMonths == 0 && durationDays == 0)
            throw new ArgumentException("Guarantee duration must be greater than zero.");

        VendorId = vendorId;
        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        DurationMonths = durationMonths;
        DurationDays = durationDays;
        Description = description?.Trim();
        TermsAndConditions = termsAndConditions?.Trim();
        IsActive = isActive;
    }

    public void Update(
        string name,
        string code,
        int durationMonths,
        string? description,
        int durationDays,
        string? termsAndConditions,
        DateTime utcNow,
        Guid? userId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        if (durationMonths < 0)
            throw new ArgumentOutOfRangeException(nameof(durationMonths), "Duration in months cannot be negative.");

        if (durationDays < 0)
            throw new ArgumentOutOfRangeException(nameof(durationDays), "Duration in days cannot be negative.");

        if (durationMonths == 0 && durationDays == 0)
            throw new ArgumentException("Guarantee duration must be greater than zero.");

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        DurationMonths = durationMonths;
        DurationDays = durationDays;
        Description = description?.Trim();
        TermsAndConditions = termsAndConditions?.Trim();

        SetUpdated(userId, utcNow);
    }

    public DateTime CalculateExpiryDate(DateTime purchaseDate)
    {
        return purchaseDate.AddMonths(DurationMonths).AddDays(DurationDays);
    }

    public void Activate(DateTime utcNow, Guid? userId = null)
    {
        IsActive = true;
        SetUpdated(userId, utcNow);
    }

    public void Deactivate(DateTime utcNow, Guid? userId = null)
    {
        IsActive = false;
        SetUpdated(userId, utcNow);
    }

    public void AddCertificate(GuaranteeCertificate certificate)
    {
        ArgumentNullException.ThrowIfNull(certificate);
        _certificates.Add(certificate);
    }
}
