using System.ComponentModel.DataAnnotations;

namespace Kahrad.Pricing.Identity.Application.DTOs.Roles;

public sealed class UpdateRoleRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; init; }
}