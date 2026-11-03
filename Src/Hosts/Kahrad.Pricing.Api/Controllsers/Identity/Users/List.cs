using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Identity.Application.DTOs.Users;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Identity.Users
{
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
    public sealed class List(IUserService userService, IAuditService auditService)
        : UsersBaseController(userService, auditService)
    {
        [HttpGet]
        [ProducesResponseType<BaseResponsePaginated<UserDto>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<BaseResponsePaginated<UserDto>>> GetUsers(
            [FromQuery] UserQueryDto filter,
            CancellationToken cancellationToken)
        {
            return Ok(await UserService.GetPagedAsync(filter, cancellationToken));
        }
    }
}
