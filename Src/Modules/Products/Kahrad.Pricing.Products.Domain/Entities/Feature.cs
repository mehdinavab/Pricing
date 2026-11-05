using Kahrad.Pricing.Products.Domain.Exceptions;
using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Kahrad.Pricing.Products.Domain.Enums;
namespace Kahrad.Pricing.Products.Domain.Entities;
public sealed class Feature : BaseEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    private Feature() { }
    public Feature(string code, string name)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new CatalogRuleException("A nonblank value is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new CatalogRuleException("A nonblank value is required.");
        if (code.Trim().Length > 100 || name.Trim().Length > 150) throw new CatalogRuleException("Definition is too long.");
        Code = code.Trim().ToUpperInvariant(); Name = name.Trim();
    }
    public void SetDefaultName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new CatalogRuleException("A nonblank value is required.");
        if (name.Trim().Length > 150) throw new CatalogRuleException("Definition name is too long.");
        Name = name.Trim();
    }
}
