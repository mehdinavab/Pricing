using System.ComponentModel.DataAnnotations;

namespace Kahrad.Pricing.Identity.Application.DTOs.Users;

public sealed class CreateUserRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Username { get; init; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string LastName { get; init; } = string.Empty;

    [EmailAddress]
    [StringLength(256)]
    public string? Email { get; init; }

    [Phone]
    [StringLength(20)]
    public required string PhoneNumber { get; init; }

    [Required]
    [StringLength(128, MinimumLength = 12)]
    public string Password { get; init; } = string.Empty;

    [MaxLength(50)]
    public IReadOnlyCollection<Guid> RoleIds { get; init; } = [];
}