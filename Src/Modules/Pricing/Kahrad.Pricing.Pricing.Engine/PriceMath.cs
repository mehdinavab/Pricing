namespace Kahrad.Pricing.Pricing.Engine;

/// <summary>Only the pricing rules explicitly confirmed by the business specification.</summary>
public static class PriceMath
{
    public static decimal RoundUpToStep(decimal amount, decimal step = 5000m)
    {
        if (amount < 0 || step <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        return decimal.Ceiling(amount / step) * step;
    }

    public static decimal ProductionCost(IEnumerable<ItemCostInput> items, long brandId, string currencyCode)
    {
        ArgumentNullException.ThrowIfNull(items);
        if (brandId <= 0) throw new ArgumentOutOfRangeException(nameof(brandId));
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);
        var hasItems = false;
        var total = 0m;
        foreach (var item in items)
        {
            hasItems = true;
            if (item.BrandId != brandId || item.UnitPrice is null || item.UnitPrice < 0 ||
                item.QuantityUsed <= 0 || !string.Equals(item.CurrencyCode, currencyCode, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Production cost has incomplete or mismatched item prices.");
            total += item.UnitPrice.Value * item.QuantityUsed;
        }
        if (!hasItems) throw new InvalidOperationException("Production cost requires priced items.");
        return total;
    }

    public static CustomerPriceResult CustomerSellingPrice(decimal representativePurchasePrice,
        decimal representativeSalesPercentage, decimal generalDiscountPercentage,
        decimal productDiscountPercentage)
    {
        if (representativePurchasePrice <= 0) throw new ArgumentOutOfRangeException(nameof(representativePurchasePrice));
        var netPercentage = representativeSalesPercentage - generalDiscountPercentage - productDiscountPercentage;
        var raw = representativePurchasePrice * (1 + netPercentage / 100m);
        if (raw < representativePurchasePrice)
            throw new InvalidOperationException("A representative cannot sell below their purchase price.");
        return new CustomerPriceResult(netPercentage, raw, RoundUpToStep(raw));
    }
}

public sealed record ItemCostInput(long BrandId, decimal? UnitPrice, decimal QuantityUsed, string CurrencyCode);
public sealed record CustomerPriceResult(decimal NetPercentage, decimal RawPrice, decimal RoundedPrice);
