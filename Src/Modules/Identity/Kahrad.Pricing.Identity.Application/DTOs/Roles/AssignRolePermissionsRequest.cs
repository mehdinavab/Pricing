using System.ComponentModel.DataAnnotations;

namespace Kahrad.Pricing.Identity.Application.DTOs.Roles;

public sealed class AssignRolePermissionsRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public IReadOnlyCollection<long> PermissionIds { get; init; } = [];
}