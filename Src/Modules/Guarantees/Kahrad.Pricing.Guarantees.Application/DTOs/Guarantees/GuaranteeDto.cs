namespace Kahrad.Pricing.Guarantees.Application.DTOs.Guarantees;

public class GuaranteeDto
{
    public long Id { get; init; }
    public Guid PublicId { get; init; }
    public long VendorId { get; init; }
    public string? VendorName { get; init; }
    public string Name { get; init; } = null!;
    public string Code { get; init; } = null!;
    public int DurationMonths { get; init; }
    public int DurationDays { get; init; }
    public string? Description { get; init; }
    public string? TermsAndConditions { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
