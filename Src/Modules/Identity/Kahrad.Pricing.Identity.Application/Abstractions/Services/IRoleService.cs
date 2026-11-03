using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Identity.Application.DTOs.Permissions;
using Kahrad.Pricing.Identity.Application.DTOs.Roles;

namespace Kahrad.Pricing.Identity.Application.Abstractions.Services
{
    public interface IRoleService
    {
        Task<RoleDto?> GetByIdAsync(
            Guid roleId,
            CancellationToken cancellationToken = default);

        Task<RoleDto?> GetByNameAsync(
            string roleName,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<RoleDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<BaseResponsePaginated<RoleDto>> GetPagedAsync(
            RoleQueryDto filter,
            CancellationToken cancellationToken = default);

        Task<Guid> CreateAsync(
            CreateRoleRequest request,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            Guid roleId,
            UpdateRoleRequest request,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid roleId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<PermissionDto>> GetPermissionsAsync(
            Guid roleId,
            CancellationToken cancellationToken = default);

        Task AssignPermissionsAsync(
            Guid roleId,
            IReadOnlyCollection<long> permissionIds,
            CancellationToken cancellationToken = default);

        Task RemovePermissionsAsync(
            Guid roleId,
            IReadOnlyCollection<long> permissionIds,
            CancellationToken cancellationToken = default);
    }
}
