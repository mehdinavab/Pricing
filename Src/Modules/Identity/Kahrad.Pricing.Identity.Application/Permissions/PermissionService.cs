using AutoMapper;
using Kahrad.Pricing.Identity.Application.Abstractions.Persistence;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Permissions;

namespace Kahrad.Pricing.Identity.Application.Permissions
{
    public sealed class PermissionService(
        IPermissionRepository permissionRepository,
        IMapper mapper) : IPermissionService
    {
        public async Task<PermissionDto?> GetByIdAsync(
            long permissionId,
            CancellationToken cancellationToken = default)
        {
            var permission = await permissionRepository.GetByIdAsync(
                permissionId,
                cancellationToken);

            return permission is null ? null : mapper.Map<PermissionDto>(permission);
        }

        public async Task<PermissionDto?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            var permission = await permissionRepository.GetByCodeAsync(
                code,
                cancellationToken);

            return permission is null ? null : mapper.Map<PermissionDto>(permission);
        }

        public async Task<IReadOnlyCollection<PermissionDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var permissions = await permissionRepository.GetAllAsync(cancellationToken);
            return mapper.Map<IReadOnlyCollection<PermissionDto>>(permissions);
        }

        public async Task<IReadOnlyCollection<PermissionDto>> GetByRoleAsync(
            Guid roleId,
            CancellationToken cancellationToken = default)
        {
            var permissions = await permissionRepository.GetByRoleIdAsync(
                roleId,
                cancellationToken);

            return mapper.Map<IReadOnlyCollection<PermissionDto>>(permissions);
        }

        public async Task<IReadOnlyCollection<PermissionDto>> GetByUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var permissions = await permissionRepository.GetByUserIdAsync(
                userId,
                cancellationToken);

            return mapper.Map<IReadOnlyCollection<PermissionDto>>(permissions);
        }

        public Task<bool> UserHasPermissionAsync(
            Guid userId,
            string permissionCode,
            CancellationToken cancellationToken = default)
        {
            return permissionRepository.UserHasPermissionAsync(
                userId,
                permissionCode,
                cancellationToken);
        }
    }
}
