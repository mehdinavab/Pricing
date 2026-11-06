using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Sms.Application.Exceptions;

public sealed class SmsNotFoundException : KahradPricingException
{
    public SmsNotFoundException(long id)
        : base($"SMS message with ID '{id}' was not found.", false, ErrorCodes.NotFound)
    {
    }

    public SmsNotFoundException(Guid publicId)
        : base($"SMS message with Public ID '{publicId}' was not found.", false, ErrorCodes.NotFound)
    {
    }
}
