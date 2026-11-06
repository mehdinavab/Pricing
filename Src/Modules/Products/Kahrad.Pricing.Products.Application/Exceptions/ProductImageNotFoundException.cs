using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Products.Application.Exceptions;

public sealed class ProductImageNotFoundException : KahradPricingException
{
    public ProductImageNotFoundException(long imageId)
        : base($"Product image with ID '{imageId}' was not found.", false, ErrorCodes.NotFound)
    {
    }

    public ProductImageNotFoundException(Guid publicId)
        : base($"Product image with Public ID '{publicId}' was not found.", false, ErrorCodes.NotFound)
    {
    }
}
