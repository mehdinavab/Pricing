using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kahrad.Pricing.Api.Controllers.Guarantees.Plans;

[Route("api/guarantees")]
[ApiController]
[Authorize]
public abstract class GuaranteesBaseController(
    IGuaranteeService guaranteeService,
    IAuditService auditService) : ControllerBase
{
    protected IGuaranteeService GuaranteeService { get; } = guaranteeService;
    protected IAuditService AuditService { get; } = auditService;

    protected Guid? CurrentUserId =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            ? userId
            : null;

    protected Task LogAuditAsync(
        string action,
        string? entityId,
        string? description,
        CancellationToken cancellationToken)
    {
        return AuditService.LogAsync(
            action,
            "Guarantee",
            entityId,
            description,
            CurrentUserId,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.TraceIdentifier,
            cancellationToken);
    }
}
