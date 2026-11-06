using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Kahrad.Pricing.Api.Controllers.Identity.Auth;

public sealed class VerifyOtp(IAuthenticationService authenticationService)
    : IdentityAuthBaseController(authenticationService)
{
    [AllowAnonymous]
    [EnableRateLimiting("otp-auth")]
    [HttpPost("otp/verify")]
    [ProducesResponseType<TokenResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<TokenResponseDto>> Verify(
        VerifyOtpRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await AuthenticationService.LoginWithOtpAsync(
                request,
                cancellationToken));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }
}
