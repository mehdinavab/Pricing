namespace Kahrad.Pricing.Identity.Application.DTOs.Roles;

public sealed class RoleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public IReadOnlyCollection<string> Permissions { get; init; } = [];
}