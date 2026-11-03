using System.Security.Claims;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Identity.Auth
{
    public sealed class Logout(IAuthenticationService authenticationService)
        : IdentityAuthBaseController(authenticationService)
    {
        [AllowAnonymous]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LogoutSession(
            RefreshTokenRequest request,
            CancellationToken cancellationToken)
        {
            await AuthenticationService.LogoutAsync(request, cancellationToken);
            return NoContent();
        }

        [Authorize]
        [HttpPost("logout-all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LogoutAll(
            CancellationToken cancellationToken)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdValue, out var userId))
                return Unauthorized();

            await AuthenticationService.LogoutAllAsync(userId, cancellationToken);
            return NoContent();
        }
    }
}
