using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kahrad.Pricing.Products.Domain.Entities;

[Table("ProductGroups", Schema = "products")]
[Index(nameof(PublicId), IsUnique = true)]
[Index(nameof(Code), IsUnique = true)]
[Index(nameof(IsDeleted))]
public sealed class ProductGroup : BaseEntity
{
    private readonly List<Product> _products = [];

    [Sieve(CanFilter = true, CanSort = true)]
    public string Name { get; private set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public string Code { get; private set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public decimal PriceEffectPercentage { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public bool IsActive { get; private set; } = true;

    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private ProductGroup()
    {
    }

    public ProductGroup(
        string name,
        string code,
        decimal priceEffectPercentage = 0,
        string? description = null,
        bool isActive = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        PriceEffectPercentage = priceEffectPercentage;
        Description = description?.Trim();
        IsActive = isActive;
    }

    public void Update(
        string name,
        string code,
        decimal priceEffectPercentage,
        string? description,
        DateTime utcNow,
        Guid? userId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        PriceEffectPercentage = priceEffectPercentage;
        Description = description?.Trim();

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

    public void AddProduct(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        _products.Add(product);
    }
}
