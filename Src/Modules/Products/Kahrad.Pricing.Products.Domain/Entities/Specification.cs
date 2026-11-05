using Kahrad.Pricing.Products.Domain.Exceptions;
using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Kahrad.Pricing.Products.Domain.Enums;
namespace Kahrad.Pricing.Products.Domain.Entities;
public sealed class Specification : BaseEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public SpecificationValueType ValueType { get; private set; }
    public string? Unit { get; private set; }
    private Specification() { }
    public Specification(string code, string name, SpecificationValueType valueType, string? unit = null)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new CatalogRuleException("A nonblank value is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new CatalogRuleException("A nonblank value is required.");
        if (code.Trim().Length > 100 || name.Trim().Length > 150) throw new CatalogRuleException("Definition is too long.");
        Code = code.Trim().ToUpperInvariant(); Name = name.Trim();
        if (!Enum.IsDefined(valueType)) throw new CatalogRuleException("Invalid value type.");
        if (unit?.Length > 50) throw new CatalogRuleException("Unit is too long.");
        ValueType = valueType; Unit = unit?.Trim();
    }
    public void SetDefaultName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new CatalogRuleException("A nonblank value is required.");
        if (name.Trim().Length > 150) throw new CatalogRuleException("Definition name is too long.");
        Name = name.Trim();
    }
}
