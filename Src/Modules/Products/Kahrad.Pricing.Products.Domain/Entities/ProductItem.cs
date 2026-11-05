using Kahrad.Pricing.BuildingBlocks.Domain.Common;

namespace Kahrad.Pricing.Products.Domain.Entities;

/// <summary>Quantity of a local item used by a product, expressed in the item's local unit.</summary>
public sealed class ProductItem : BaseEntity
{
    private ProductItem() { }

    public ProductItem(long productId, long itemId, decimal quantityUsed)
    {
        if (productId <= 0 || itemId <= 0) throw new ArgumentOutOfRangeException(nameof(productId));
        SetQuantity(quantityUsed);
        ProductId = productId;
        ItemId = itemId;
    }

    public long ProductId { get; private set; }
    public long ItemId { get; private set; }
    public decimal QuantityUsed { get; private set; }

    public void SetQuantity(decimal quantityUsed)
    {
        if (quantityUsed <= 0 || decimal.Round(quantityUsed, 4) != quantityUsed)
            throw new ArgumentOutOfRangeException(nameof(quantityUsed), "Item quantity must be positive with at most four decimal places.");
        QuantityUsed = quantityUsed;
    }
}
