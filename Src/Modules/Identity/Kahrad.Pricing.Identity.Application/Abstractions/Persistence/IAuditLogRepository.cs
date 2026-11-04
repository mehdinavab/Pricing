using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Identity.Application.DTOs.Auditing;
using Kahrad.Pricing.Identity.Domain.Entities;

namespace Kahrad.Pricing.Identity.Application.Abstractions.Persistence
{
    public interface IAuditLogRepository
    {
        Task AddAsync(
            AuditLog auditLog,
            CancellationToken cancellationToken = default);

        Task<BaseResponsePaginated<AuditLog>> GetPagedAsync(
            AuditLogQueryDto filter,
            CancellationToken cancellationToken = default);

        Task<AuditLog?> GetByIdAsync(
            long id,
            CancellationToken cancellationToken = default);
    }
}
