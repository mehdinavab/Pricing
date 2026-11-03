using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Users;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Identity.Users
{
        [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
        public sealed class Create(
            IUserService userService,
            IAuditService auditService,
            IRoleService roleService)
            : UsersBaseController(userService, auditService)
    {
        [HttpPost]
        [ProducesResponseType<Guid>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> CreateUser(
            CreateUserRequest request,
            CancellationToken cancellationToken)
        {
            if (!User.IsInRole(RoleConstants.SuperAdmin))
            {
                foreach (var roleId in request.RoleIds.Distinct())
                {
                    var role = await roleService.GetByIdAsync(
                        roleId,
                        cancellationToken);

                    if (role?.Name == RoleConstants.SuperAdmin)
                        return Forbid();
                }
            }

            var userId = await UserService.CreateAsync(request, cancellationToken);
            await LogAuditAsync(
                "UserCreated",
                "User",
                userId.ToString(),
                "User account created.",
                cancellationToken);

            return Created($"/api/users/{userId}", userId);
        }
    }
}
