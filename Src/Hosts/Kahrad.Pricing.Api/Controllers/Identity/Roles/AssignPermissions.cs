using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Roles;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Identity.Roles
{
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public sealed class AssignPermissions(
        IRoleService roleService,
        IAuditService auditService)
        : RolesBaseController(roleService, auditService)
    {
        [HttpPost("{roleId:guid}/permissions")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignRolePermissions(
            Guid roleId,
            AssignRolePermissionsRequest request,
            CancellationToken cancellationToken)
        {
            await RoleService.AssignPermissionsAsync(
                roleId,
                request.PermissionIds,
                cancellationToken);
            await LogAuditAsync(
                "RolePermissionsAssigned",
                roleId,
                "Permissions assigned to role.",
                cancellationToken);

            return NoContent();
        }

        [HttpDelete("{roleId:guid}/permissions")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveRolePermissions(
            Guid roleId,
            AssignRolePermissionsRequest request,
            CancellationToken cancellationToken)
        {
            await RoleService.RemovePermissionsAsync(
                roleId,
                request.PermissionIds,
                cancellationToken);
            await LogAuditAsync(
                "RolePermissionsRemoved",
                roleId,
                "Permissions removed from role.",
                cancellationToken);

            return NoContent();
        }
    }
}
