using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kahrad.Pricing.Api.Controllers.Brands.Stores;

[Route("api/stores")]
[ApiController]
[Authorize]
public abstract class StoresBaseController(
    IStoreService storeService,
    IAuditService auditService) : ControllerBase
{
    protected IStoreService StoreService { get; } = storeService;
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
            "Store",
            entityId,
            description,
            CurrentUserId,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.TraceIdentifier,
            cancellationToken);
    }
}
