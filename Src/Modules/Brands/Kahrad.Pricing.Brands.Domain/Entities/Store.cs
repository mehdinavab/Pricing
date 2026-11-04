using Kahrad.Pricing.Brands.Domain.Enums;
using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kahrad.Pricing.Brands.Domain.Entities;

[Table("Stores", Schema = "brands")]
[Index(nameof(PublicId), IsUnique = true)]
[Index(nameof(BrandId), nameof(Code), IsUnique = true)]
[Index(nameof(BrandId))]
[Index(nameof(IsDeleted))]
public sealed class Store : BaseEntity
{
    public long BrandId { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string Name { get; private set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public string Code { get; private set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public string OwnerName { get; private set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public string? OwnerPhoneNumber { get; private set; }

    public string? OwnerNationalCode { get; private set; }

    public string? OwnerEmail { get; private set; }

    public string? PictureUrl { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string Address { get; private set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public string? City { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string? Province { get; private set; }

    public string? PostalCode { get; private set; }

    public string? Phone { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public PriceAdjustmentType PriceAdjustmentType { get; private set; } = PriceAdjustmentType.None;

    [Sieve(CanFilter = true, CanSort = true)]
    public decimal PriceAdjustmentPercentage { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public bool IsActive { get; private set; } = true;

    public Brand Brand { get; private set; } = null!;

    private Store()
    {
    }

    public Store(
        long brandId,
        string name,
        string code,
        string ownerName,
        string address,
        string? ownerPhoneNumber = null,
        string? ownerNationalCode = null,
        string? ownerEmail = null,
        string? pictureUrl = null,
        string? city = null,
        string? province = null,
        string? postalCode = null,
        string? phone = null,
        PriceAdjustmentType priceAdjustmentType = PriceAdjustmentType.None,
        decimal priceAdjustmentPercentage = 0m,
        string? description = null,
        bool isActive = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(address);

        if (priceAdjustmentPercentage < 0 || priceAdjustmentPercentage > 100)
            throw new ArgumentOutOfRangeException(nameof(priceAdjustmentPercentage), "Percentage must be between 0 and 100.");

        BrandId = brandId;
        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        OwnerName = ownerName.Trim();
        Address = address.Trim();
        OwnerPhoneNumber = ownerPhoneNumber?.Trim();
        OwnerNationalCode = ownerNationalCode?.Trim();
        OwnerEmail = ownerEmail?.Trim();
        PictureUrl = pictureUrl?.Trim();
        City = city?.Trim();
        Province = province?.Trim();
        PostalCode = postalCode?.Trim();
        Phone = phone?.Trim();
        PriceAdjustmentType = priceAdjustmentType;
        PriceAdjustmentPercentage = priceAdjustmentPercentage;
        Description = description?.Trim();
        IsActive = isActive;
    }

    public void Update(
        string name,
        string code,
        string ownerName,
        string address,
        string? ownerPhoneNumber,
        string? ownerNationalCode,
        string? ownerEmail,
        string? pictureUrl,
        string? city,
        string? province,
        string? postalCode,
        string? phone,
        string? description,
        DateTime utcNow,
        Guid? userId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(address);

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        OwnerName = ownerName.Trim();
        Address = address.Trim();
        OwnerPhoneNumber = ownerPhoneNumber?.Trim();
        OwnerNationalCode = ownerNationalCode?.Trim();
        OwnerEmail = ownerEmail?.Trim();
        PictureUrl = pictureUrl?.Trim();
        City = city?.Trim();
        Province = province?.Trim();
        PostalCode = postalCode?.Trim();
        Phone = phone?.Trim();
        Description = description?.Trim();

        SetUpdated(userId, utcNow);
    }

    public void SetPriceAdjustment(
        PriceAdjustmentType type,
        decimal percentage,
        DateTime utcNow,
        Guid? userId = null)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentOutOfRangeException(nameof(percentage), "Percentage must be between 0 and 100.");

        PriceAdjustmentType = type;
        PriceAdjustmentPercentage = type == PriceAdjustmentType.None ? 0m : percentage;
        SetUpdated(userId, utcNow);
    }

    public void SetPicture(string? pictureUrl, DateTime utcNow, Guid? userId = null)
    {
        PictureUrl = pictureUrl?.Trim();
        SetUpdated(userId, utcNow);
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
}
