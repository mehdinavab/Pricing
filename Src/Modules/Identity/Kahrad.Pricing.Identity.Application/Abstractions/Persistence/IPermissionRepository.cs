using Kahrad.Pricing.Identity.Domain.Entities;

namespace Kahrad.Pricing.Identity.Application.Abstractions.Persistence
{
    public interface IPermissionRepository
    {
        Task<Permission?> GetByIdAsync(
            long id,
            CancellationToken cancellationToken = default);

        Task<Permission?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<Permission>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<Permission>> GetByRoleIdAsync(
            Guid roleId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<Permission>> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            long permissionId,
            CancellationToken cancellationToken = default);

        Task AssignToRoleAsync(
            Guid roleId,
            IReadOnlyCollection<long> permissionIds,
            CancellationToken cancellationToken = default);

        Task RemoveFromRoleAsync(
            Guid roleId,
            IReadOnlyCollection<long> permissionIds,
            CancellationToken cancellationToken = default);

        Task<bool> UserHasPermissionAsync(
            Guid userId,
            string permissionCode,
            CancellationToken cancellationToken = default);
    }
}
