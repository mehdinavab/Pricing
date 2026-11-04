namespace Kahrad.Pricing.Products.Application.Exceptions;

public sealed class ProductNotFoundException : KeyNotFoundException
{
    public ProductNotFoundException(long productId)
        : base($"Product with ID '{productId}' was not found.")
    {
    }

    public ProductNotFoundException(Guid publicId)
        : base($"Product with Public ID '{publicId}' was not found.")
    {
    }

    public ProductNotFoundException(string code)
        : base($"Product with code '{code}' was not found.")
    {
    }
}
