using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Identity.Users
{
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
    public sealed class GetRoles(IUserService userService, IAuditService auditService)
        : UsersBaseController(userService, auditService)
    {
        [HttpGet("{userId:guid}/roles")]
        [ProducesResponseType<IReadOnlyCollection<string>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyCollection<string>>> GetUserRoles(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return Ok(await UserService.GetUserRolesAsync(userId, cancellationToken));
        }
    }
}
