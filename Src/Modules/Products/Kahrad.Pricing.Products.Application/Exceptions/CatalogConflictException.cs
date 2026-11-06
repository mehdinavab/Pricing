using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Products.Application.Exceptions;
public sealed class CatalogConflictException(string message)
    : KahradPricingException(message, false, ErrorCodes.Conflict);
