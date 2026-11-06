using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Guarantees.Application.Exceptions;

public class GuaranteeOperationException : KahradPricingException
{
    public GuaranteeOperationException(string message)
        : base(message, false, ErrorCodes.BadRequest)
    {
    }

    public GuaranteeOperationException(string message, Exception innerException)
        : base(message, innerException, false, ErrorCodes.BadRequest)
    {
    }
}
