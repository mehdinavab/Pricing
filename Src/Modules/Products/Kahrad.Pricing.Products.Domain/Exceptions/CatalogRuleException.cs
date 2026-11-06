using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Products.Domain.Exceptions;
public sealed class CatalogRuleException(string message, string? parameter = null)
    : KahradPricingException(message, false, ErrorCodes.BadRequest)
{
    public string? ParameterName { get; } = parameter;
}
