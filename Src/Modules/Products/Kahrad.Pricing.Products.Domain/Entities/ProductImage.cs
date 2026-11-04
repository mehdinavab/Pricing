using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kahrad.Pricing.Products.Domain.Entities;

[Table("ProductImages", Schema = "products")]
[Index(nameof(PublicId), IsUnique = true)]
[Index(nameof(ProductId))]
[Index(nameof(ProductId), nameof(IsPrimary))]
[Index(nameof(IsDeleted))]
public sealed class ProductImage : BaseEntity
{
    public long ProductId { get; private set; }

    public string ImageUrl { get; private set; } = null!;

    [Sieve(CanFilter = true, CanSort = true)]
    public string? AltText { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public int DisplayOrder { get; private set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public bool IsPrimary { get; private set; }

    public Product Product { get; private set; } = null!;

    private ProductImage()
    {
    }

    public ProductImage(
        long productId,
        string imageUrl,
        string? altText = null,
        int displayOrder = 0,
        bool isPrimary = false,
        string? description = null)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be positive.");

        ArgumentException.ThrowIfNullOrWhiteSpace(imageUrl);

        ProductId = productId;
        ImageUrl = imageUrl.Trim();
        AltText = altText?.Trim();
        DisplayOrder = displayOrder;
        IsPrimary = isPrimary;
        Description = description?.Trim();
    }

    public void Update(
        string imageUrl,
        string? altText,
        int displayOrder,
        string? description,
        DateTime utcNow,
        Guid? userId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(imageUrl);

        ImageUrl = imageUrl.Trim();
        AltText = altText?.Trim();
        DisplayOrder = displayOrder;
        Description = description?.Trim();

        SetUpdated(userId, utcNow);
    }

    public void SetPrimary(bool isPrimary, DateTime utcNow, Guid? userId = null)
    {
        IsPrimary = isPrimary;
        SetUpdated(userId, utcNow);
    }
}
