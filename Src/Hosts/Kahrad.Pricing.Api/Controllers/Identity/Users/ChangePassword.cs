using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Users;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kahrad.Pricing.Api.Controllers.Identity.Users
{
    public sealed class ChangePassword(
        IUserService userService,
        IAuditService auditService)
        : UsersBaseController(userService, auditService)
    {
        [HttpPost("{userId:guid}/change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ChangeUserPassword(
            Guid userId,
            ChangePasswordRequest request,
            CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(currentUserId, out var parsedUserId) ||
                parsedUserId != userId)
            {
                return Forbid();
            }

            await UserService.ChangePasswordAsync(
                userId,
                request,
                cancellationToken);
            await LogAuditAsync(
                "PasswordChanged",
                "User",
                userId.ToString(),
                "User password changed.",
                cancellationToken);

            return NoContent();
        }
    }
}
