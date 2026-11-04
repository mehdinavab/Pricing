namespace Kahrad.Pricing.Products.Application.Exceptions;

public sealed class ProductImageNotFoundException : KeyNotFoundException
{
    public ProductImageNotFoundException(long imageId)
        : base($"Product image with ID '{imageId}' was not found.")
    {
    }

    public ProductImageNotFoundException(Guid publicId)
        : base($"Product image with Public ID '{publicId}' was not found.")
    {
    }
}
