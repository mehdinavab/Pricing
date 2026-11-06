using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Brands.Application.Exceptions;

public class BrandOperationException : KahradPricingException
{
    public BrandOperationException(string message)
        : base(message, false, ErrorCodes.BadRequest)
    {
    }

    public BrandOperationException(string message, Exception innerException)
        : base(message, innerException, false, ErrorCodes.BadRequest)
    {
    }
}
