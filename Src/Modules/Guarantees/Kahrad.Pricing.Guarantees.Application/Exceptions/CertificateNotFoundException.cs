namespace Kahrad.Pricing.Guarantees.Application.Exceptions;

public sealed class CertificateNotFoundException : KeyNotFoundException
{
    public CertificateNotFoundException(long certificateId)
        : base($"Guarantee certificate with ID '{certificateId}' was not found.")
    {
    }

    public CertificateNotFoundException(string certificateNumber)
        : base($"Guarantee certificate with number '{certificateNumber}' was not found.")
    {
    }

    public CertificateNotFoundException(Guid publicId)
        : base($"Guarantee certificate with Public ID '{publicId}' was not found.")
    {
    }
}
