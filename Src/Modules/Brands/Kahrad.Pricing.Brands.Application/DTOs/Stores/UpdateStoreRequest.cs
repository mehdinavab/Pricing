namespace Kahrad.Pricing.Brands.Application.DTOs.Stores;

public sealed class UpdateStoreRequest
{
    public string Name { get; init; } = null!;
    public string Code { get; init; } = null!;
    public string OwnerName { get; init; } = null!;
    public string? OwnerPhoneNumber { get; init; }
    public string? OwnerNationalCode { get; init; }
    public string? OwnerEmail { get; init; }
    public string? PictureUrl { get; init; }
    public string Address { get; init; } = null!;
    public string? City { get; init; }
    public string? Province { get; init; }
    public string? PostalCode { get; init; }
    public string? Phone { get; init; }
    public string? Description { get; init; }
}
