using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Roles;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Identity.Roles;

[Authorize(Roles = RoleConstants.SuperAdmin)]
public sealed class Update(
    IRoleService roleService,
    IAuditService auditService)
    : RolesBaseController(roleService, auditService)
{
    [HttpPut("{roleId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRole(
        Guid roleId,
        UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        await RoleService.UpdateAsync(roleId, request, cancellationToken);
        await LogAuditAsync(
            "RoleUpdated",
            roleId,
            "Role updated.",
            cancellationToken);
        return NoContent();
    }
}
