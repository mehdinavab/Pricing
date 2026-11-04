using Sieve.Models;

namespace Kahrad.Pricing.Guarantees.Application.DTOs.Guarantees;

public sealed class GuaranteeQueryDto : SieveModel
{
    public long? VendorId { get; init; }
}
