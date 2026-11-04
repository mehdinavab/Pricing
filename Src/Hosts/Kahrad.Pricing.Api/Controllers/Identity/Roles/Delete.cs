using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Identity.Roles;

[Authorize(Roles = RoleConstants.SuperAdmin)]
public sealed class Delete(
    IRoleService roleService,
    IAuditService auditService)
    : RolesBaseController(roleService, auditService)
{
    [HttpDelete("{roleId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRole(
        Guid roleId,
        CancellationToken cancellationToken)
    {
        await RoleService.DeleteAsync(roleId, cancellationToken);
        await LogAuditAsync(
            "RoleDeleted",
            roleId,
            "Role deleted.",
            cancellationToken);
        return NoContent();
    }
}
