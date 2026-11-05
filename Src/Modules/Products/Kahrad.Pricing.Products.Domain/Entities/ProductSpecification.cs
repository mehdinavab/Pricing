using Kahrad.Pricing.Products.Domain.Exceptions;
using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Kahrad.Pricing.Products.Domain.Enums;
namespace Kahrad.Pricing.Products.Domain.Entities;
public sealed class ProductSpecification : BaseEntity
{
    public long ProductId { get; private set; }
    public long SpecificationId { get; private set; }
    public string? TextValue { get; private set; }
    public decimal? NumberValue { get; private set; }
    public bool? BooleanValue { get; private set; }
    public DateTime? DateValue { get; private set; }
    public string? CodeValue { get; private set; }
    public int DisplayOrder { get; private set; }
    private ProductSpecification() { }
    public ProductSpecification(long productId, long specificationId)
    {
        if (productId <= 0 || specificationId <= 0) throw new CatalogRuleException("Assignment IDs must be positive.");
        ProductId = productId; SpecificationId = specificationId;
    }
    public void EnsureTextual(Specification definition)
    {
        if (definition.Id != SpecificationId || definition.ValueType != SpecificationValueType.Text)
            throw new CatalogRuleException("Only textual specification values can be translated.");
    }
    public void SetValue(SpecificationValueType type, string? text, decimal? number, bool? boolean,
        DateTime? date, string? code, int displayOrder)
    {
        var count = (text is null ? 0 : 1) + (number.HasValue ? 1 : 0) + (boolean.HasValue ? 1 : 0)
            + (date.HasValue ? 1 : 0) + (code is null ? 0 : 1);
        if (count != 1 || type switch
        {
            SpecificationValueType.Text => string.IsNullOrWhiteSpace(text) || text.Length > 500,
            SpecificationValueType.Number => !number.HasValue,
            SpecificationValueType.Boolean => !boolean.HasValue,
            SpecificationValueType.Date => !date.HasValue,
            SpecificationValueType.Code => string.IsNullOrWhiteSpace(code) || code.Length > 100,
            _ => true
        }) throw new CatalogRuleException("Supply exactly one value matching the specification type.");
        if (number.HasValue && (number.Value <= -100000000000000m || number.Value >= 100000000000000m || decimal.Round(number.Value, 4) != number.Value))
            throw new CatalogRuleException("Numeric specification values must fit decimal(18,4) without rounding.");
        if (date.HasValue && date.Value.TimeOfDay != TimeSpan.Zero)
            throw new CatalogRuleException("Date specification values must not contain a time of day.");
        TextValue = text?.Trim(); NumberValue = number; BooleanValue = boolean; DateValue = date;
        CodeValue = code?.Trim(); DisplayOrder = displayOrder;
    }
}
