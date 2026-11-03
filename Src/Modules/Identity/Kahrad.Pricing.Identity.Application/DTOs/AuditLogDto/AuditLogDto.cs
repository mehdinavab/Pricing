namespace Kahrad.Pricing.Identity.Application.DTOs.Auditing;

public sealed class AuditLogDto
{
    public long Id { get; init; }
    public Guid? UserId { get; init; }
    public string? Username { get; init; }
    public string Action { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public string? EntityId { get; init; }
    public string? Description { get; init; }
    public string? IpAddress { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}