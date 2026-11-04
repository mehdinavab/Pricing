using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Identity.Auth
{
    public sealed class Refresh(IAuthenticationService authenticationService)
        : IdentityAuthBaseController(authenticationService)
    {
        [AllowAnonymous]
        [HttpPost("refresh")]
        [ProducesResponseType<TokenResponseDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(
            RefreshTokenRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await AuthenticationService.RefreshTokenAsync(
                    request,
                    cancellationToken));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}
