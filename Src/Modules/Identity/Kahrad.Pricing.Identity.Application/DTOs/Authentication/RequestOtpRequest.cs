using System.ComponentModel.DataAnnotations;

namespace Kahrad.Pricing.Identity.Application.DTOs.Authentication;

public sealed class RequestOtpRequest
{
    [Required]
    [Phone]
    [StringLength(20)]
    public string PhoneNumber { get; init; } = string.Empty;
}