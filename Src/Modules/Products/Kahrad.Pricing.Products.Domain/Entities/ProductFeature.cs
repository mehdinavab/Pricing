using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kahrad.Pricing.Products.Domain.Entities;

[Table("ProductFeatures", Schema = "products")]
[Index(nameof(PublicId), IsUnique = true)]
[Index(nameof(ProductId), nameof(Name))]
[Index(nameof(ProductId))]
[Index(nameof(HasPriceEffect))]
[Index(nameof(IsDeleted))]
public sealed class ProductFeature : BaseEntity
{
    public long? FeatureId { get; private set; }

    public void AssignDefinition(long featureId)
    {
        if (featureId <= 0) throw new ArgumentOutOfRangeException(nameof(featureId));
        FeatureId = featureId;
    }

    public long ProductId { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string Name { get; private set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public string Value { get; private set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public bool HasPriceEffect { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public decimal PriceEffectPercentage { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public decimal PriceEffectAmount { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public int DisplayOrder { get; private set; }

    public Product Product { get; private set; } = null!;

    private ProductFeature()
    {
    }

    public ProductFeature(
        long productId,
        string name,
        string value,
        bool hasPriceEffect,
        decimal priceEffectPercentage = 0,
        decimal priceEffectAmount = 0,
        int displayOrder = 0,
        string? description = null)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be positive.");

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        ProductId = productId;
        Name = name.Trim();
        Value = value.Trim();
        HasPriceEffect = hasPriceEffect;
        PriceEffectPercentage = hasPriceEffect ? priceEffectPercentage : 0;
        PriceEffectAmount = hasPriceEffect ? priceEffectAmount : 0;
        DisplayOrder = displayOrder;
        Description = description?.Trim();
    }

    public void Update(
        string name,
        string value,
        bool hasPriceEffect,
        decimal priceEffectPercentage,
        decimal priceEffectAmount,
        int displayOrder,
        string? description,
        DateTime utcNow,
        Guid? userId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Name = name.Trim();
        Value = value.Trim();
        HasPriceEffect = hasPriceEffect;
        PriceEffectPercentage = hasPriceEffect ? priceEffectPercentage : 0;
        PriceEffectAmount = hasPriceEffect ? priceEffectAmount : 0;
        DisplayOrder = displayOrder;
        Description = description?.Trim();

        SetUpdated(userId, utcNow);
    }
}
