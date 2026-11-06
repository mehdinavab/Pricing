using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Representatives.Application;

public sealed class RepresentativeOperationException(string message)
    : KahradPricingException(message, false, ErrorCodes.BadRequest);
