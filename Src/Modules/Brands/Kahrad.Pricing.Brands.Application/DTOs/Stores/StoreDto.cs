using Kahrad.Pricing.Brands.Domain.Enums;

namespace Kahrad.Pricing.Brands.Application.DTOs.Stores;

public class StoreDto
{
    public long Id { get; init; }
    public Guid PublicId { get; init; }
    public long BrandId { get; init; }
    public string? BrandName { get; init; }
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
    public PriceAdjustmentType PriceAdjustmentType { get; init; }
    public decimal PriceAdjustmentPercentage { get; init; }
    public bool IsActive { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
