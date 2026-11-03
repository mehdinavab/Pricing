using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Identity.Users
{
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
    public sealed class RemoveRole(IUserService userService, IAuditService auditService)
        : UsersBaseController(userService, auditService)
    {
        [HttpDelete("{userId:guid}/roles/{roleId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveUserRole(
            Guid userId,
            Guid roleId,
            CancellationToken cancellationToken)
        {
            await UserService.RemoveRoleAsync(userId, roleId, cancellationToken);
            await LogAuditAsync(
                "UserRoleRemoved",
                "User",
                userId.ToString(),
                $"Role '{roleId}' removed.",
                cancellationToken);

            return NoContent();
        }
    }
}
