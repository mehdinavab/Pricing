namespace Kahrad.Pricing.Pricing.Application;

public sealed class PricingOperationException(string message, bool isConflict = false) : Exception(message)
{
    public bool IsConflict { get; } = isConflict;
}
