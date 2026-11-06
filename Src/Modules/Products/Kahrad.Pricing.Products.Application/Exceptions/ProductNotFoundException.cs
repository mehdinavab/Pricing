using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Products.Application.Exceptions;

public sealed class ProductNotFoundException : KahradPricingException
{
    public ProductNotFoundException(long productId)
        : base($"Product with ID '{productId}' was not found.", false, ErrorCodes.NotFound)
    {
    }

    public ProductNotFoundException(Guid publicId)
        : base($"Product with Public ID '{publicId}' was not found.", false, ErrorCodes.NotFound)
    {
    }

    public ProductNotFoundException(string code)
        : base($"Product with code '{code}' was not found.", false, ErrorCodes.NotFound)
    {
    }
}
