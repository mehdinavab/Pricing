using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Roles;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Identity.Roles
{
    public sealed class GetById(
        IRoleService roleService,
        IAuditService auditService)
        : RolesBaseController(roleService, auditService)
    {
        [HttpGet("{roleId:guid}")]
        [ProducesResponseType<RoleDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleDto>> GetRole(
            Guid roleId,
            CancellationToken cancellationToken)
        {
            var role = await RoleService.GetByIdAsync(roleId, cancellationToken);
            return role is null ? NotFound() : Ok(role);
        }
    }
}
