using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Pricing.Application;

public sealed class PricingPolicyConflictException(string message,
    IReadOnlyList<long> affectedProductIds)
    : KahradPricingException(message, false, ErrorCodes.Conflict)
{
    public IReadOnlyList<long> AffectedProductIds { get; } = affectedProductIds;
}
