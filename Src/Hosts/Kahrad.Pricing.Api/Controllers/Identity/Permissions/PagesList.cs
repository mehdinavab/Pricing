using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Identity.Permissions
{
    public sealed class PagesList(
        IPermissionService permissionService,
        IAuditService auditService)
        : PermissionsBaseController(permissionService, auditService)
    {
        [HttpGet]
        [ProducesResponseType<IReadOnlyCollection<PermissionDto>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyCollection<PermissionDto>>> GetPermissions(
            CancellationToken cancellationToken)
        {
            return Ok(await PermissionService.GetAllAsync(cancellationToken));
        }
    }
}
