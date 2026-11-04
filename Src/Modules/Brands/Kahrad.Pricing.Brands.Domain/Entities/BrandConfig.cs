using Kahrad.Pricing.Brands.Domain.Enums;
using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kahrad.Pricing.Brands.Domain.Entities;

[Table("BrandConfigs", Schema = "brands")]
[Index(nameof(PublicId), IsUnique = true)]
[Index(nameof(BrandId), nameof(Key), IsUnique = true)]
[Index(nameof(BrandId))]
public sealed class BrandConfig : BaseEntity
{
    public long BrandId { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string Key { get; private set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public string DisplayName { get; private set; } = null!;

    public string Value { get; private set; } = null!;

    public BrandConfigDataType DataType { get; private set; } = BrandConfigDataType.String;

    [Sieve(CanFilter = true, CanSort = true)]
    public string? Category { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public bool IsActive { get; private set; } = true;

    public Brand Brand { get; private set; } = null!;

    private BrandConfig()
    {
    }

    public BrandConfig(
        long brandId,
        string key,
        string displayName,
        string value,
        BrandConfigDataType dataType = BrandConfigDataType.String,
        string? category = null,
        string? description = null,
        bool isActive = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentNullException.ThrowIfNull(value);

        if (brandId < 0)
            throw new ArgumentOutOfRangeException(nameof(brandId));

        BrandId = brandId;
        Key = key.Trim();
        DisplayName = displayName.Trim();
        Value = value.Trim();
        DataType = dataType;
        Category = category?.Trim();
        Description = description?.Trim();
        IsActive = isActive;
    }

    public void Update(
        string displayName,
        string value,
        BrandConfigDataType dataType,
        string? category,
        string? description,
        bool isActive,
        DateTime utcNow,
        Guid? userId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentNullException.ThrowIfNull(value);

        DisplayName = displayName.Trim();
        Value = value.Trim();
        DataType = dataType;
        Category = category?.Trim();
        Description = description?.Trim();
        IsActive = isActive;

        SetUpdated(userId, utcNow);
    }

    public void SetValue(string value, DateTime utcNow, Guid? userId = null)
    {
        ArgumentNullException.ThrowIfNull(value);
        Value = value.Trim();
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
