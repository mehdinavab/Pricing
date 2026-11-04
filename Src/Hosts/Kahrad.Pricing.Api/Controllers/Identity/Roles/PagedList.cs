using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Roles;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Identity.Roles
{
    public sealed class PagedList(
        IRoleService roleService,
        IAuditService auditService)
        : RolesBaseController(roleService, auditService)
    {
        [HttpGet]
        [ProducesResponseType<BaseResponsePaginated<RoleDto>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<BaseResponsePaginated<RoleDto>>> GetRoles(
            [FromQuery] RoleQueryDto filter,
            CancellationToken cancellationToken)
        {
            return Ok(await RoleService.GetPagedAsync(filter, cancellationToken));
        }
    }
}
