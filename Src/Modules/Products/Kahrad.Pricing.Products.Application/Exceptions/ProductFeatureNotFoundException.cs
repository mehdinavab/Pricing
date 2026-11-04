namespace Kahrad.Pricing.Products.Application.Exceptions;

public sealed class ProductFeatureNotFoundException : KeyNotFoundException
{
    public ProductFeatureNotFoundException(long featureId)
        : base($"Product feature with ID '{featureId}' was not found.")
    {
    }

    public ProductFeatureNotFoundException(Guid publicId)
        : base($"Product feature with Public ID '{publicId}' was not found.")
    {
    }
}
