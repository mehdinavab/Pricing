using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Identity.Users
{
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
    public sealed class AssignRole(
        IUserService userService,
        IAuditService auditService,
        IRoleService roleService)
        : UsersBaseController(userService, auditService)
    {
        [HttpPost("{userId:guid}/roles/{roleId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignUserRole(
            Guid userId,
            Guid roleId,
            CancellationToken cancellationToken)
        {
            if (!User.IsInRole(RoleConstants.SuperAdmin))
            {
                var role = await roleService.GetByIdAsync(roleId, cancellationToken);
                if (role?.Name == RoleConstants.SuperAdmin)
                    return Forbid();
            }

            await UserService.AssignRoleAsync(userId, roleId, cancellationToken);
            await LogAuditAsync(
                "UserRoleAssigned",
                "User",
                userId.ToString(),
                $"Role '{roleId}' assigned.",
                cancellationToken);

            return NoContent();
        }
    }
}
