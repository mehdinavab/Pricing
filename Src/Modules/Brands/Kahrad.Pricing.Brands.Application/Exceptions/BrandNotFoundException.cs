namespace Kahrad.Pricing.Brands.Application.Exceptions;

public sealed class BrandNotFoundException : KeyNotFoundException
{
    public BrandNotFoundException(long brandId)
        : base($"Brand with ID '{brandId}' was not found.")
    {
    }

    public BrandNotFoundException(Guid publicId)
        : base($"Brand with Public ID '{publicId}' was not found.")
    {
    }

    public BrandNotFoundException(string code)
        : base($"Brand with code '{code}' was not found.")
    {
    }
}
