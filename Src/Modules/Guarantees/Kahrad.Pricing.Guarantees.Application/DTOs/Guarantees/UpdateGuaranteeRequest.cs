namespace Kahrad.Pricing.Guarantees.Application.DTOs.Guarantees;

public sealed class UpdateGuaranteeRequest
{
    public string Name { get; init; } = null!;
    public string Code { get; init; } = null!;
    public int DurationMonths { get; init; }
    public int DurationDays { get; init; }
    public string? Description { get; init; }
    public string? TermsAndConditions { get; init; }
}
