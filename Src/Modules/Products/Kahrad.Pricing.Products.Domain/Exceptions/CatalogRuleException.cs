namespace Kahrad.Pricing.Products.Domain.Exceptions;
public sealed class CatalogRuleException(string message, string? parameter = null) : Exception(message)
{
    public string? ParameterName { get; } = parameter;
}
