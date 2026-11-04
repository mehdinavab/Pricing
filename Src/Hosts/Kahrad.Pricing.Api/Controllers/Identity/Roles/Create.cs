using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Roles;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Identity.Roles
{
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public sealed class Create(
        IRoleService roleService,
        IAuditService auditService)
        : RolesBaseController(roleService, auditService)
    {
        [HttpPost]
        [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> CreateRole(
            CreateRoleRequest request,
            CancellationToken cancellationToken)
        {
            var roleId = await RoleService.CreateAsync(request, cancellationToken);
            await LogAuditAsync(
                "RoleCreated",
                roleId,
                "Role created.",
                cancellationToken);

            return Created($"/api/roles/{roleId}", roleId);
        }
    }
}
