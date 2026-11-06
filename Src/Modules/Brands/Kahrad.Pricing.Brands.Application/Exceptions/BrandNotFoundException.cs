using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Brands.Application.Exceptions;

public sealed class BrandNotFoundException : KahradPricingException
{
    public BrandNotFoundException(long brandId)
        : base($"Brand with ID '{brandId}' was not found.", false, ErrorCodes.NotFound)
    {
    }

    public BrandNotFoundException(Guid publicId)
        : base($"Brand with Public ID '{publicId}' was not found.", false, ErrorCodes.NotFound)
    {
    }

    public BrandNotFoundException(string code)
        : base($"Brand with code '{code}' was not found.", false, ErrorCodes.NotFound)
    {
    }
}
