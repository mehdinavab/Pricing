using System.ComponentModel.DataAnnotations;

namespace Kahrad.Pricing.Identity.Application.DTOs.Users;

public sealed class ChangePasswordRequest
{
    [Required]
    public string CurrentPassword { get; init; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 12)]
    public string NewPassword { get; init; } = string.Empty;
}