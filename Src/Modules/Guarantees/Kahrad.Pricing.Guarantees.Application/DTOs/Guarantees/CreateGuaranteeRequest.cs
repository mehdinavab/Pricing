namespace Kahrad.Pricing.Guarantees.Application.DTOs.Guarantees;

public sealed class CreateGuaranteeRequest
{
    public long VendorId { get; init; }
    public string Name { get; init; } = null!;
    public string Code { get; init; } = null!;
    public int DurationMonths { get; init; }
    public int DurationDays { get; init; } = 0;
    public string? Description { get; init; }
    public string? TermsAndConditions { get; init; }
    public bool IsActive { get; init; } = true;
}
