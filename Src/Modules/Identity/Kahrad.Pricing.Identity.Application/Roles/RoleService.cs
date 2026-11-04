using AutoMapper;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Identity.Application.Abstractions.Persistence;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Permissions;
using Kahrad.Pricing.Identity.Application.DTOs.Roles;
using Kahrad.Pricing.Identity.Application.Exceptions;
using Kahrad.Pricing.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace Kahrad.Pricing.Identity.Application.Roles
{
    public sealed class RoleService(
        RoleManager<ApplicationRole> roleManager,
        IPermissionRepository permissionRepository,
        IMapper mapper,
        ISieveProcessor sieveProcessor) : IRoleService
    {
        public async Task<RoleDto?> GetByIdAsync(
            Guid roleId,
            CancellationToken cancellationToken = default)
        {
            var role = await roleManager.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == roleId, cancellationToken);

            return role is null ? null : await MapRoleAsync(role, cancellationToken);
        }

        public async Task<RoleDto?> GetByNameAsync(
            string roleName,
            CancellationToken cancellationToken = default)
        {
            var role = await roleManager.FindByNameAsync(roleName);

            return role is null ? null : await MapRoleAsync(role, cancellationToken);
        }

        public async Task<IReadOnlyCollection<RoleDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var roles = await roleManager.Roles
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);

            var result = new List<RoleDto>(roles.Count);
            foreach (var role in roles)
                result.Add(await MapRoleAsync(role, cancellationToken));

            return result;
        }

        public async Task<BaseResponsePaginated<RoleDto>> GetPagedAsync(
            RoleQueryDto filter,
            CancellationToken cancellationToken = default)
        {
            EnsureStableSort(filter);

            var query = roleManager.Roles.AsNoTracking();
            var totalCount = await sieveProcessor
                .Apply(
                    filter,
                    query,
                    applyFiltering: true,
                    applySorting: false,
                    applyPagination: false)
                .CountAsync(cancellationToken);

            var roles = await sieveProcessor
                .Apply(
                    filter,
                    query,
                    applyFiltering: true,
                    applySorting: true,
                    applyPagination: true)
                .ToListAsync(cancellationToken);

            var data = new List<RoleDto>(roles.Count);
            foreach (var role in roles)
                data.Add(await MapRoleAsync(role, cancellationToken));

            return BaseResponsePaginated<RoleDto>.Success(
                data,
                filter.Page ?? 1,
                filter.PageSize ?? 20,
                totalCount);
        }

        public async Task<Guid> CreateAsync(
            CreateRoleRequest request,
            CancellationToken cancellationToken = default)
        {
            var role = new ApplicationRole(request.Name, request.Description);
            EnsureSucceeded(await roleManager.CreateAsync(role));

            return role.Id;
        }

        public async Task UpdateAsync(
            Guid roleId,
            UpdateRoleRequest request,
            CancellationToken cancellationToken = default)
        {
            var role = await FindRoleAsync(roleId);
            role.Name = request.Name.Trim();
            role.UpdateDescription(request.Description);

            EnsureSucceeded(await roleManager.UpdateAsync(role));
        }

        public async Task DeleteAsync(
            Guid roleId,
            CancellationToken cancellationToken = default)
        {
            var role = await FindRoleAsync(roleId);
            EnsureSucceeded(await roleManager.DeleteAsync(role));
        }

        public async Task<IReadOnlyCollection<PermissionDto>> GetPermissionsAsync(
            Guid roleId,
            CancellationToken cancellationToken = default)
        {
            _ = await FindRoleAsync(roleId);

            var permissions = await permissionRepository.GetByRoleIdAsync(
                roleId,
                cancellationToken);

            return mapper.Map<IReadOnlyCollection<PermissionDto>>(permissions);
        }

        public async Task AssignPermissionsAsync(
            Guid roleId,
            IReadOnlyCollection<long> permissionIds,
            CancellationToken cancellationToken = default)
        {
            _ = await FindRoleAsync(roleId);
            await ValidatePermissionsAsync(permissionIds, cancellationToken);
            await permissionRepository.AssignToRoleAsync(
                roleId,
                permissionIds,
                cancellationToken);
        }

        public async Task RemovePermissionsAsync(
            Guid roleId,
            IReadOnlyCollection<long> permissionIds,
            CancellationToken cancellationToken = default)
        {
            _ = await FindRoleAsync(roleId);
            await ValidatePermissionsAsync(permissionIds, cancellationToken);
            await permissionRepository.RemoveFromRoleAsync(
                roleId,
                permissionIds,
                cancellationToken);
        }

        private async Task<RoleDto> MapRoleAsync(
            ApplicationRole role,
            CancellationToken cancellationToken)
        {
            var permissions = await permissionRepository.GetByRoleIdAsync(
                role.Id,
                cancellationToken);
            var result = mapper.Map<RoleDto>(role);
            return new RoleDto
            {
                Id = result.Id,
                Name = result.Name,
                Description = result.Description,
                Permissions = permissions.Select(x => x.Code).ToArray()
            };
        }

        private async Task<ApplicationRole> FindRoleAsync(Guid roleId)
        {
            return await roleManager.FindByIdAsync(roleId.ToString())
                ?? throw new KeyNotFoundException("Role not found.");
        }

        private async Task ValidatePermissionsAsync(
            IReadOnlyCollection<long> permissionIds,
            CancellationToken cancellationToken)
        {
            var requestedIds = permissionIds.Distinct().ToArray();
            foreach (var permissionId in requestedIds)
            {
                if (!await permissionRepository.ExistsAsync(
                    permissionId,
                    cancellationToken))
                {
                    throw new KeyNotFoundException(
                        $"Permission '{permissionId}' was not found.");
                }
            }
        }

        private static void EnsureStableSort(RoleQueryDto filter)
        {
            var sortFields = filter.Sorts?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => x.TrimStart('+', '-'));

            if (sortFields?.Contains("Id", StringComparer.OrdinalIgnoreCase) == true)
                return;

            filter.Sorts = string.IsNullOrWhiteSpace(filter.Sorts)
                ? "Name,Id"
                : $"{filter.Sorts},Id";
        }

        private static void EnsureSucceeded(IdentityResult result)
        {
            if (result.Succeeded)
                return;

            throw new IdentityOperationException(string.Join(
                "; ",
                result.Errors.Select(x => $"{x.Code}: {x.Description}")));
        }
    }
}
