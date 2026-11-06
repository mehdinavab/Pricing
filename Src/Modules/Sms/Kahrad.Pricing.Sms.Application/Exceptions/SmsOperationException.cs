using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Sms.Application.Exceptions;

public class SmsOperationException : KahradPricingException
{
    public SmsOperationException(string message)
        : base(message, false, ErrorCodes.BadRequest)
    {
    }

    public SmsOperationException(string message, Exception innerException)
        : base(message, innerException, false, ErrorCodes.BadRequest)
    {
    }
}
