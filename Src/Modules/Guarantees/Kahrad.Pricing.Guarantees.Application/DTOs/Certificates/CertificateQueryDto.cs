using Kahrad.Pricing.Guarantees.Domain.Enums;
using Sieve.Models;

namespace Kahrad.Pricing.Guarantees.Application.DTOs.Certificates;

public sealed class CertificateQueryDto : SieveModel
{
    public long? ProductId { get; init; }
    public long? GuaranteeId { get; init; }
    public long? StoreId { get; init; }
    public GuaranteeCertificateStatus? Status { get; init; }
}
