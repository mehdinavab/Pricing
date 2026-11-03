using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Identity.Application.DTOs.Auditing;

namespace Kahrad.Pricing.Identity.Application.Abstractions.Services
{
    public interface IAuditService
    {
        Task<AuditLogDto?> GetByIdAsync(
            long id,
            CancellationToken cancellationToken = default);

        Task<BaseResponsePaginated<AuditLogDto>> GetPagedAsync(
            AuditLogQueryDto filter,
            CancellationToken cancellationToken = default);

        Task LogAsync(
            string action,
            string entityType,
            string? entityId = null,
            string? description = null,
            Guid? actorUserId = null,
            string? ipAddress = null,
            string? correlationId = null,
            CancellationToken cancellationToken = default);
    }
}
