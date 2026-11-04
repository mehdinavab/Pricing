namespace Kahrad.Pricing.Products.Application.Exceptions;

public sealed class ProductGroupNotFoundException : KeyNotFoundException
{
    public ProductGroupNotFoundException(long groupId)
        : base($"Product group with ID '{groupId}' was not found.")
    {
    }

    public ProductGroupNotFoundException(Guid publicId)
        : base($"Product group with Public ID '{publicId}' was not found.")
    {
    }

    public ProductGroupNotFoundException(string code)
        : base($"Product group with code '{code}' was not found.")
    {
    }
}
