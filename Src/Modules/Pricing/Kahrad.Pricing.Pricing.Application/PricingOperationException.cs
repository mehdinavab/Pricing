using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Pricing.Application;

public sealed class PricingOperationException(string message, bool isConflict = false)
    : KahradPricingException(
        message,
        false,
        isConflict ? ErrorCodes.Conflict : ErrorCodes.BadRequest)
{
    public bool IsConflict { get; } = isConflict;
}
