using Kahrad.Pricing.Identity.Application.Abstractions.Persistence;
using Kahrad.Pricing.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Identity.Infrastructure.Persistence.Repositories;

public sealed class PermissionRepository(IdentityDbContext context)
    : IPermissionRepository
{
    public async Task<Permission?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await context.Permissions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Permission?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await context.Permissions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Permission>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await context.Permissions
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Permission>> GetByRoleIdAsync(
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        return await (
            from permission in context.Permissions.AsNoTracking()
            join rolePermission in context.RolePermissions.AsNoTracking()
                on permission.Id equals rolePermission.PermissionId
            where rolePermission.RoleId == roleId
            orderby permission.Code
            select permission
        ).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Permission>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await (
            from permission in context.Permissions.AsNoTracking()
            join rolePermission in context.RolePermissions.AsNoTracking()
                on permission.Id equals rolePermission.PermissionId
            join userRole in context.UserRoles.AsNoTracking()
                on rolePermission.RoleId equals userRole.RoleId
            where userRole.UserId == userId
            orderby permission.Code
            select permission
        )
        .Distinct()
        .ToListAsync(cancellationToken);
    }

    public async Task<bool> UserHasPermissionAsync(
        Guid userId,
        string permissionCode,
        CancellationToken cancellationToken = default)
    {
        return await (
            from permission in context.Permissions
            join rolePermission in context.RolePermissions
                on permission.Id equals rolePermission.PermissionId
            join userRole in context.UserRoles
                on rolePermission.RoleId equals userRole.RoleId
            where userRole.UserId == userId &&
                  permission.Code == permissionCode
            select permission.Id
        ).AnyAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        long permissionId,
        CancellationToken cancellationToken = default)
    {
        return await context.Permissions
            .AnyAsync(x => x.Id == permissionId, cancellationToken);
    }

    public async Task AssignToRoleAsync(
        Guid roleId,
        IReadOnlyCollection<long> permissionIds,
        CancellationToken cancellationToken = default)
    {
        if (permissionIds.Count == 0)
            return;

        var requestedIds = permissionIds.Distinct().ToArray();

        var existingIds = await context.RolePermissions
            .Where(x =>
                x.RoleId == roleId &&
                requestedIds.Contains(x.PermissionId))
            .Select(x => x.PermissionId)
            .ToListAsync(cancellationToken);

        var newAssignments = requestedIds
            .Except(existingIds)
            .Select(permissionId => new RolePermission(roleId, permissionId))
            .ToList();

        if (newAssignments.Count == 0)
            return;

        await context.RolePermissions.AddRangeAsync(
            newAssignments,
            cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveFromRoleAsync(
        Guid roleId,
        IReadOnlyCollection<long> permissionIds,
        CancellationToken cancellationToken = default)
    {
        if (permissionIds.Count == 0)
            return;

        var requestedIds = permissionIds.Distinct().ToArray();

        await context.RolePermissions
            .Where(x =>
                x.RoleId == roleId &&
                requestedIds.Contains(x.PermissionId))
            .ExecuteDeleteAsync(cancellationToken);
    }
}