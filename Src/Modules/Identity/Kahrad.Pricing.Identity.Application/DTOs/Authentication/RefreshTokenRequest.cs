using System.ComponentModel.DataAnnotations;

namespace Kahrad.Pricing.Identity.Application.DTOs.Authentication;

public sealed class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; init; } = string.Empty;
}