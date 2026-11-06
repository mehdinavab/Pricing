using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Identity.Application.Exceptions;

public sealed class IdentityOperationException(string message)
    : KahradPricingException(message, false, ErrorCodes.BadRequest);
