using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Products.Application.Exceptions;

public sealed class ProductGroupNotFoundException : KahradPricingException
{
    public ProductGroupNotFoundException(long groupId)
        : base($"Product group with ID '{groupId}' was not found.", false, ErrorCodes.NotFound)
    {
    }

    public ProductGroupNotFoundException(Guid publicId)
        : base($"Product group with Public ID '{publicId}' was not found.", false, ErrorCodes.NotFound)
    {
    }

    public ProductGroupNotFoundException(string code)
        : base($"Product group with code '{code}' was not found.", false, ErrorCodes.NotFound)
    {
    }
}
