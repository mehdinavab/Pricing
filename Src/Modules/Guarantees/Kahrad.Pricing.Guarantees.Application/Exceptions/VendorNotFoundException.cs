using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Guarantees.Application.Exceptions;

public sealed class VendorNotFoundException : KahradPricingException
{
    public VendorNotFoundException(long vendorId)
        : base($"Vendor with ID '{vendorId}' was not found.", false, ErrorCodes.NotFound)
    {
    }

    public VendorNotFoundException(string code)
        : base($"Vendor with code '{code}' was not found.", false, ErrorCodes.NotFound)
    {
    }

    public VendorNotFoundException(Guid publicId)
        : base($"Vendor with Public ID '{publicId}' was not found.", false, ErrorCodes.NotFound)
    {
    }
}
