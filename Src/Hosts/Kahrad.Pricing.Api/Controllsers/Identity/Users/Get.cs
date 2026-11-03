using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Users;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Identity.Users
{
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
    public sealed class Get(IUserService userService, IAuditService auditService)
        : UsersBaseController(userService, auditService)
    {
        [HttpGet("{userId:guid}")]
        [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetUser(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var user = await UserService.GetByIdAsync(userId, cancellationToken);
            return user is null ? NotFound() : Ok(user);
        }
    }
}
