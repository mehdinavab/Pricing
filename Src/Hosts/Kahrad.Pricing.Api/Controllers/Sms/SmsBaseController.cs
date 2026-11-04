using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kahrad.Pricing.Api.Controllers.Sms;

[Route("api/sms")]
[ApiController]
[Authorize]
public abstract class SmsBaseController(
    ISmsService smsService,
    IAuditService auditService) : ControllerBase
{
    protected ISmsService SmsService { get; } = smsService;
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
            "Sms",
            entityId,
            description,
            CurrentUserId,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.TraceIdentifier,
            cancellationToken);
    }
}
