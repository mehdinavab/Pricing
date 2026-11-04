using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kahrad.Pricing.Api.Controllers.Guarantees.Vendors;

[Route("api/guarantee-vendors")]
[ApiController]
[Authorize]
public abstract class VendorsBaseController(
    IVendorService vendorService,
    IAuditService auditService) : ControllerBase
{
    protected IVendorService VendorService { get; } = vendorService;
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
            "GuaranteeVendor",
            entityId,
            description,
            CurrentUserId,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.TraceIdentifier,
            cancellationToken);
    }
}
