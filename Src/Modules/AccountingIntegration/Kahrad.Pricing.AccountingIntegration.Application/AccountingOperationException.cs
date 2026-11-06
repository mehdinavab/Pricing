using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.AccountingIntegration.Application;

public sealed class AccountingOperationException(string message)
    : KahradPricingException(message, false, ErrorCodes.BadRequest);
