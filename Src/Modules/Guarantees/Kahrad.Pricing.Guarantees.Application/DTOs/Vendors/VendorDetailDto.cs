using Kahrad.Pricing.Guarantees.Application.DTOs.Guarantees;

namespace Kahrad.Pricing.Guarantees.Application.DTOs.Vendors;

public sealed class VendorDetailDto : VendorDto
{
    public IReadOnlyCollection<GuaranteeDto> Guarantees { get; init; } = [];
}
