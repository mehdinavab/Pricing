using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kahrad.Pricing.Api.Controllsers.Identity.Roles
{
    [Route("api/roles")]
    [ApiController]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
    public abstract class RolesBaseController : ControllerBase
    {
        protected RolesBaseController(
            IRoleService roleService,
            IAuditService auditService)
        {
            RoleService = roleService;
            AuditService = auditService;
        }

        protected IRoleService RoleService { get; }
        protected IAuditService AuditService { get; }

        protected Task LogAuditAsync(
            string action,
            Guid roleId,
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
                "Role",
                roleId.ToString(),
                description,
                actorUserId,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.TraceIdentifier,
                cancellationToken);
        }
    }
}
