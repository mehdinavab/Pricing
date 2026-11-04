using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Users;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Identity.Users
{
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
    public sealed class Update(IUserService userService, IAuditService auditService)
        : UsersBaseController(userService, auditService)
    {
        [HttpPut("{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser(
            Guid userId,
            UpdateUserRequest request,
            CancellationToken cancellationToken)
        {
            await UserService.UpdateAsync(userId, request, cancellationToken);
            await LogAuditAsync(
                "UserUpdated",
                "User",
                userId.ToString(),
                "User profile updated.",
                cancellationToken);

            return NoContent();
        }
    }
}
