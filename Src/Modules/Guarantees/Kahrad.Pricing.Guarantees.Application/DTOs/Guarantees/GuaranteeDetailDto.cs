namespace Kahrad.Pricing.Guarantees.Application.DTOs.Guarantees;

public sealed class GuaranteeDetailDto : GuaranteeDto
{
    public int ActiveCertificateCount { get; init; }
    public int TotalCertificateCount { get; init; }
}
