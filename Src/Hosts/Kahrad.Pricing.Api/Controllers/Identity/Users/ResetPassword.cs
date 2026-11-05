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
    public sealed class ResetPassword(
        IUserService userService,
        IAuditService auditService)
        : UsersBaseController(userService, auditService)
    {
        [HttpPost("{userId:guid}/reset-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetUserPassword(
            Guid userId,
            ResetPasswordRequest request,
            CancellationToken cancellationToken)
        {
            await UserService.ResetPasswordAsync(
                userId,
                request,
                cancellationToken);
            await LogAuditAsync(
                "PasswordReset",
                "User",
                userId.ToString(),
                "User password reset by an administrator.",
                cancellationToken);

            return NoContent();
        }
    }
}
