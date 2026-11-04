namespace Kahrad.Pricing.Guarantees.Application.DTOs.Certificates;

public sealed class RevokeCertificateRequest
{
    public string Reason { get; init; } = null!;
}
