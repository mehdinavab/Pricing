using System.ComponentModel.DataAnnotations;

namespace Kahrad.Pricing.Identity.Application.DTOs.Authentication;

public sealed class VerifyOtpRequest
{
    [Required]
    public Guid RequestId { get; init; }

    [Required]
    [Phone]
    [StringLength(20)]
    public string PhoneNumber { get; init; } = string.Empty;

    [Required]
    [StringLength(10, MinimumLength = 4)]
    public string Code { get; init; } = string.Empty;
}
