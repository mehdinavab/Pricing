using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Identity.Auth
{
    [Route("api/identity/auth")]
    [ApiController]
    public abstract class IdentityAuthBaseController(
        IAuthenticationService authenticationService) : ControllerBase
    {
        protected IAuthenticationService AuthenticationService { get; } =
            authenticationService;
    }
}
