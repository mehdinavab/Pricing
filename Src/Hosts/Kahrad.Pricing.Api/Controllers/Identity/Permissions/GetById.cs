using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Identity.Permissions
{
    public sealed class GetById(
        IPermissionService permissionService,
        IAuditService auditService)
        : PermissionsBaseController(permissionService, auditService)
    {
        [HttpGet("{permissionId:long}")]
        [ProducesResponseType<PermissionDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PermissionDto>> GetPermission(
            long permissionId,
            CancellationToken cancellationToken)
        {
            var permission = await PermissionService.GetByIdAsync(
                permissionId,
                cancellationToken);

            return permission is null ? NotFound() : Ok(permission);
        }
    }
}
