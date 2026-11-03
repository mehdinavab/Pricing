using System.ComponentModel.DataAnnotations;

namespace Kahrad.Pricing.Identity.Application.DTOs.Users;

public sealed class UpdateUserRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string LastName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public required string Email { get; init; }

    [Required]
    [Phone]
    [StringLength(20)]
    public required string PhoneNumber { get; init; }
}