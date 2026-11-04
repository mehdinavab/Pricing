namespace Kahrad.Pricing.Guarantees.Application.Exceptions;

public sealed class VendorNotFoundException : KeyNotFoundException
{
    public VendorNotFoundException(long vendorId)
        : base($"Vendor with ID '{vendorId}' was not found.")
    {
    }

    public VendorNotFoundException(string code)
        : base($"Vendor with code '{code}' was not found.")
    {
    }

    public VendorNotFoundException(Guid publicId)
        : base($"Vendor with Public ID '{publicId}' was not found.")
    {
    }
}
