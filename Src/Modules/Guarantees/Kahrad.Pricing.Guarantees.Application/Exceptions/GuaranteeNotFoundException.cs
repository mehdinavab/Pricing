namespace Kahrad.Pricing.Guarantees.Application.Exceptions;

public sealed class GuaranteeNotFoundException : KeyNotFoundException
{
    public GuaranteeNotFoundException(long guaranteeId)
        : base($"Guarantee with ID '{guaranteeId}' was not found.")
    {
    }

    public GuaranteeNotFoundException(long vendorId, string code)
        : base($"Guarantee with code '{code}' for Vendor '{vendorId}' was not found.")
    {
    }

    public GuaranteeNotFoundException(Guid publicId)
        : base($"Guarantee with Public ID '{publicId}' was not found.")
    {
    }
}
