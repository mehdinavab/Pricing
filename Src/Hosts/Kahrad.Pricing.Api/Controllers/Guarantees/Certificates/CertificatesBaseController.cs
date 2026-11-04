using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kahrad.Pricing.Api.Controllers.Guarantees.Certificates;

[Route("api/guarantee-certificates")]
[ApiController]
[Authorize]
public abstract class CertificatesBaseController(
    IGuaranteeCertificateService certificateService,
    IAuditService auditService) : ControllerBase
{
    protected IGuaranteeCertificateService CertificateService { get; } = certificateService;
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
            "GuaranteeCertificate",
            entityId,
            description,
            CurrentUserId,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.TraceIdentifier,
            cancellationToken);
    }
}
