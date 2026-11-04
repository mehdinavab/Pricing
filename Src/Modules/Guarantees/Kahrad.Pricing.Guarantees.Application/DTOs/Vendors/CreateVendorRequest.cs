namespace Kahrad.Pricing.Guarantees.Application.DTOs.Vendors;

public sealed class CreateVendorRequest
{
    public string Name { get; init; } = null!;
    public string Code { get; init; } = null!;
    public string? ContactPerson { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public string? Website { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
}
