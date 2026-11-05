using Kahrad.Pricing.Api.Authorization;
﻿using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Users;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Identity.Users
{
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
    [ServiceFilter(typeof(ProtectSuperAdminFilter))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public sealed class ChangeStatus(IUserService userService, IAuditService auditService)
        : UsersBaseController(userService, auditService)
    {
        [HttpPatch("{userId:guid}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetStatus(
            Guid userId,
            ChangeUserStatusRequest request,
            CancellationToken cancellationToken)
        {
            await UserService.SetActiveStatusAsync(
                userId,
                request.IsActive,
                cancellationToken);
            await LogAuditAsync(
                request.IsActive ? "UserActivated" : "UserDeactivated",
                "User",
                userId.ToString(),
                "User active status changed.",
                cancellationToken);

            return NoContent();
        }
    }
}
