using Kahrad.Pricing.Identity.Application.DTOs.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Identity.Auth
{
    public sealed class LoginEndpoint(
        Kahrad.Pricing.Identity.Application.Abstractions.Services.IAuthenticationService authenticationService)
        : IdentityAuthBaseController(authenticationService)
    {
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType<TokenResponseDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TokenResponseDto>> Login(
            PasswordLoginRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await AuthenticationService.LoginWithPasswordAsync(
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
