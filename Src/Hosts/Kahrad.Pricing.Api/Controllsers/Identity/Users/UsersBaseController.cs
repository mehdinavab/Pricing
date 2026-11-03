using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kahrad.Pricing.Api.Controllsers.Identity.Users
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public abstract class UsersBaseController(
        IUserService userService,
        IAuditService auditService)
        : ControllerBase
    {
        protected IUserService UserService { get; } = userService;
        protected IAuditService AuditService { get; } = auditService;

        protected Task LogAuditAsync(
            string action,
            string entityType,
            string? entityId,
            string? description,
            CancellationToken cancellationToken)
        {
            Guid? actorUserId = Guid.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId)
                ? userId
                : null;

            return AuditService.LogAsync(
                action,
                entityType,
                entityId,
                description,
                actorUserId,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.TraceIdentifier,
                cancellationToken);
        }
    }
}
