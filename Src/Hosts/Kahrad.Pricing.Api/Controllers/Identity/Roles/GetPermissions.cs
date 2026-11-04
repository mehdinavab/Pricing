using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Identity.Roles
{
    public sealed class GetPermissions(
        IRoleService roleService,
        IAuditService auditService)
        : RolesBaseController(roleService, auditService)
    {
        [HttpGet("{roleId:guid}/permissions")]
        [ProducesResponseType<IReadOnlyCollection<PermissionDto>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyCollection<PermissionDto>>> GetRolePermissions(
            Guid roleId,
            CancellationToken cancellationToken)
        {
            return Ok(await RoleService.GetPermissionsAsync(roleId, cancellationToken));
        }
    }
}
