using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Products.Application.Exceptions;

public sealed class ProductFeatureNotFoundException : KahradPricingException
{
    public ProductFeatureNotFoundException(long featureId)
        : base($"Product feature with ID '{featureId}' was not found.", false, ErrorCodes.NotFound)
    {
    }

    public ProductFeatureNotFoundException(Guid publicId)
        : base($"Product feature with Public ID '{publicId}' was not found.", false, ErrorCodes.NotFound)
    {
    }
}
