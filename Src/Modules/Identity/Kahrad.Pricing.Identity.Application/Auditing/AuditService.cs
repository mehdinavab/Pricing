using AutoMapper;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Identity.Application.Abstractions.Persistence;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Auditing;
using Kahrad.Pricing.Identity.Domain.Entities;

namespace Kahrad.Pricing.Identity.Application.Auditing
{
    public sealed class AuditService(
        IAuditLogRepository auditLogRepository,
        IMapper mapper) : IAuditService
    {
        public async Task<AuditLogDto?> GetByIdAsync(
            long id,
            CancellationToken cancellationToken = default)
        {
            var auditLog = await auditLogRepository.GetByIdAsync(id, cancellationToken);
            return auditLog is null ? null : mapper.Map<AuditLogDto>(auditLog);
        }

        public async Task<BaseResponsePaginated<AuditLogDto>> GetPagedAsync(
            AuditLogQueryDto filter,
            CancellationToken cancellationToken = default)
        {
            var result = await auditLogRepository.GetPagedAsync(
                filter,
                cancellationToken);

            return BaseResponsePaginated<AuditLogDto>.Success(
                mapper.Map<IReadOnlyCollection<AuditLogDto>>(result.Data),
                result.Page,
                result.PageSize,
                result.TotalCount,
                result.Message);
        }

        public Task LogAsync(
            string action,
            string entityType,
            string? entityId = null,
            string? description = null,
            Guid? actorUserId = null,
            string? ipAddress = null,
            string? correlationId = null,
            CancellationToken cancellationToken = default)
        {
            Guid? entityPublicId = Guid.TryParse(entityId, out var publicId)
                ? publicId
                : null;

            var auditLog = new AuditLog(
                action,
                actorUserId,
                DateTime.UtcNow,
                entityType,
                entityPublicId,
                ipAddress: ipAddress,
                correlationId: correlationId);
            auditLog.Description = description;

            return auditLogRepository.AddAsync(auditLog, cancellationToken);
        }
    }
}
