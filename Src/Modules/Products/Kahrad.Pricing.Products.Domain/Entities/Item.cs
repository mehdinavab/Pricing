using Kahrad.Pricing.BuildingBlocks.Domain.Common;

namespace Kahrad.Pricing.Products.Domain.Entities;

/// <summary>A locally managed pricing component. Accounting catalog records never replace it.</summary>
public sealed class Item : BaseEntity
{
    private Item() { }

    public Item(string code, string name, string unitCode)
    {
        SetDetails(code, name, unitCode);
    }

    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string UnitCode { get; private set; } = null!;

    public void SetDetails(string code, string name, string unitCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(unitCode);
        if (code.Trim().Length > 100 || name.Trim().Length > 250 || unitCode.Trim().Length > 32)
            throw new ArgumentException("Item details are too long.");
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        UnitCode = unitCode.Trim().ToUpperInvariant();
    }
}
