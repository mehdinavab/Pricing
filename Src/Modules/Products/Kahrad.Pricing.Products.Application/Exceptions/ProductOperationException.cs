using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Products.Application.Exceptions;

public class ProductOperationException : KahradPricingException
{
    public ProductOperationException(string message)
        : base(message, false, ErrorCodes.BadRequest)
    {
    }

    public ProductOperationException(string message, Exception innerException)
        : base(message, innerException, false, ErrorCodes.BadRequest)
    {
    }
}
