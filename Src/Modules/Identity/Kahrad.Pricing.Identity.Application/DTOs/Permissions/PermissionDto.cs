namespace Kahrad.Pricing.Identity.Application.DTOs.Permissions;

public sealed class PermissionDto
{
    public long Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}