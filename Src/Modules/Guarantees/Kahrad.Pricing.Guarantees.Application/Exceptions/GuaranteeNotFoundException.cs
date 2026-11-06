using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Guarantees.Application.Exceptions;

public sealed class GuaranteeNotFoundException : KahradPricingException
{
    public GuaranteeNotFoundException(long guaranteeId)
        : base($"Guarantee with ID '{guaranteeId}' was not found.", false, ErrorCodes.NotFound)
    {
    }

    public GuaranteeNotFoundException(long vendorId, string code)
        : base($"Guarantee with code '{code}' for Vendor '{vendorId}' was not found.", false, ErrorCodes.NotFound)
    {
    }

    public GuaranteeNotFoundException(Guid publicId)
        : base($"Guarantee with Public ID '{publicId}' was not found.", false, ErrorCodes.NotFound)
    {
    }
}
