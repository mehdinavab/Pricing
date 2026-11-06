using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.Guarantees.Application.Exceptions;

public sealed class CertificateNotFoundException : KahradPricingException
{
    public CertificateNotFoundException(long certificateId)
        : base($"Guarantee certificate with ID '{certificateId}' was not found.", false, ErrorCodes.NotFound)
    {
    }

    public CertificateNotFoundException(string certificateNumber)
        : base($"Guarantee certificate with number '{certificateNumber}' was not found.", false, ErrorCodes.NotFound)
    {
    }

    public CertificateNotFoundException(Guid publicId)
        : base($"Guarantee certificate with Public ID '{publicId}' was not found.", false, ErrorCodes.NotFound)
    {
    }
}
