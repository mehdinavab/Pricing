using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kahrad.Pricing.Products.Domain.Entities;

[Table("Products", Schema = "products")]
[Index(nameof(PublicId), IsUnique = true)]
[Index(nameof(Code), IsUnique = true)]
[Index(nameof(GroupId))]
[Index(nameof(BrandId))]
[Index(nameof(IsDeleted))]
public sealed class Product : BaseEntity
{
    private readonly List<ProductFeature> _features = [];
    private readonly List<ProductImage> _gallery = [];

    [Sieve(CanFilter = true, CanSort = true)]
    public string Name { get; private set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public string Code { get; private set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public long GroupId { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public long? BrandId { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public decimal BasePrice { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public decimal BeautyFactorPercentage { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public decimal CommercialFactorPercentage { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public decimal SalesFactorPercentage { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public bool IsActive { get; private set; } = true;

    public ProductGroup Group { get; private set; } = null!;

    public IReadOnlyCollection<ProductFeature> Features => _features.AsReadOnly();

    public IReadOnlyCollection<ProductImage> Gallery => _gallery.AsReadOnly();

    private Product()
    {
    }

    public Product(
        string name,
        string code,
        long groupId,
        decimal basePrice,
        long? brandId = null,
        decimal beautyFactorPercentage = 0,
        decimal commercialFactorPercentage = 0,
        decimal salesFactorPercentage = 0,
        string? description = null,
        bool isActive = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        if (groupId <= 0)
            throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");

        if (basePrice < 0)
            throw new ArgumentOutOfRangeException(nameof(basePrice), "Base price cannot be negative.");

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        GroupId = groupId;
        BrandId = brandId;
        BasePrice = basePrice;
        BeautyFactorPercentage = beautyFactorPercentage;
        CommercialFactorPercentage = commercialFactorPercentage;
        SalesFactorPercentage = salesFactorPercentage;
        Description = description?.Trim();
        IsActive = isActive;
    }

    public void Update(
        string name,
        string code,
        long groupId,
        long? brandId,
        decimal basePrice,
        string? description,
        DateTime utcNow,
        Guid? userId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        if (groupId <= 0)
            throw new ArgumentOutOfRangeException(nameof(groupId), "Group ID must be positive.");

        if (basePrice < 0)
            throw new ArgumentOutOfRangeException(nameof(basePrice), "Base price cannot be negative.");

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        GroupId = groupId;
        BrandId = brandId;
        BasePrice = basePrice;
        Description = description?.Trim();

        SetUpdated(userId, utcNow);
    }

    public void UpdatePricingFactors(
        decimal beautyFactorPercentage,
        decimal commercialFactorPercentage,
        decimal salesFactorPercentage,
        DateTime utcNow,
        Guid? userId = null)
    {
        BeautyFactorPercentage = beautyFactorPercentage;
        CommercialFactorPercentage = commercialFactorPercentage;
        SalesFactorPercentage = salesFactorPercentage;

        SetUpdated(userId, utcNow);
    }

    public void SetBasePrice(decimal basePrice, DateTime utcNow, Guid? userId = null)
    {
        if (basePrice < 0)
            throw new ArgumentOutOfRangeException(nameof(basePrice), "Base price cannot be negative.");

        BasePrice = basePrice;
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

    public void AddFeature(ProductFeature feature)
    {
        ArgumentNullException.ThrowIfNull(feature);
        _features.Add(feature);
    }

    public void AddImage(ProductImage image)
    {
        ArgumentNullException.ThrowIfNull(image);
        _gallery.Add(image);
    }
}
