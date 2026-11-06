using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Kahrad.Pricing.Api.Controllers.Identity.Auth;

public sealed class RequestOtp(IAuthenticationService authenticationService)
    : IdentityAuthBaseController(authenticationService)
{
    [AllowAnonymous]
    [EnableRateLimiting("otp-auth")]
    [HttpPost("otp/request")]
    [ProducesResponseType<OtpRequestResponse>(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<OtpRequestResponse>> RequestCode(
        RequestOtpRequest request,
        CancellationToken cancellationToken)
    {
        var response = await AuthenticationService.RequestOtpAsync(
            request,
            cancellationToken);
        return Accepted(response);
    }
}
