using System.Security.Claims;
using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Kahrad.Pricing.AccountingIntegration.Application.Services;
using Kahrad.Pricing.AccountingIntegration.Domain.Entities;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Accounting;

public sealed record AccountingItemSearchRequest(long BrandId, string? Search = null, int Page = 1, int PageSize = 20);
public sealed record MapAccountingItemRequest(long BrandId, long LocalItemId, long AccountingItemId, decimal UnitConversionFactor = 1);
public sealed record UnmapAccountingItemRequest(long BrandId, long LocalItemId);
public sealed record StartAccountingSyncRequest(long BrandId);
public sealed record AccountingSyncHistoryRequest(long BrandId, AccountingSyncType? Type = null, int Page = 1, int PageSize = 20);
public sealed record AccountingInvoiceSearchRequest(long BrandId, string? Search = null, int Page = 1, int PageSize = 20);
public sealed record SaveAccountingConnectionRequest(long BrandId, string? BaseUrl, string? ApiKeySecretReference);

[ApiController]
[Route("api/accounting")]
[Authorize(Roles = RoleConstants.SuperAdmin)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public sealed class AccountingItemsController(AccountingItemMappingService mappings,
    AccountingSyncService sync, AccountingConnectionService connections,
    IAccountingInvoiceReader invoices, IAccountingSyncRunReader syncRuns, IAuditService audit) : ControllerBase
{
    [HttpPost("connections/list")]
    [ProducesResponseType<IReadOnlyList<AccountingConnectionView>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Connections(CancellationToken ct) => Ok(await connections.ListAsync(ct));

    [HttpPost("connections/save")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SaveConnection([FromBody] SaveAccountingConnectionRequest request, CancellationToken ct)
    {
        await connections.SaveAsync(request.BrandId, request.BaseUrl, request.ApiKeySecretReference, ct);
        var actor = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : (Guid?)null;
        await audit.LogAsync("AccountingConnectionSaved", "AccountingBrandConnection", request.BrandId.ToString(),
            null, actor, HttpContext.Connection.RemoteIpAddress?.ToString(), HttpContext.TraceIdentifier, ct);
        return NoContent();
    }
    [HttpPost("invoices/search")]
    [ProducesResponseType<IReadOnlyList<AccountingInvoiceView>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchInvoices([FromBody] AccountingInvoiceSearchRequest request, CancellationToken ct)
    {
        if (request.BrandId <= 0 || request.Page is < 1 or > 1000000 || request.PageSize is < 1 or > 100)
            return BadRequest();
        return Ok(await invoices.SearchAsync(request.BrandId, request.Search?.Trim(), request.Page, request.PageSize, ct));
    }
    [HttpPost("items/search")]
    [ProducesResponseType<IReadOnlyList<AccountingItemView>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromBody] AccountingItemSearchRequest request, CancellationToken ct) =>
        Ok(await mappings.SearchItemsAsync(request.BrandId, request.Search, request.Page, request.PageSize, ct));

    [HttpPost("items/map")]
    [ProducesResponseType<long>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Map([FromBody] MapAccountingItemRequest request, CancellationToken ct)
    {
        var actor = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : (Guid?)null;
        var mappingId = await mappings.MapAsync(request.BrandId, request.LocalItemId,
            request.AccountingItemId, request.UnitConversionFactor, actor, ct);
        await audit.LogAsync("AccountingItemMapped", "AccountingItemMapping", mappingId.ToString(),
            null, actor, HttpContext.Connection.RemoteIpAddress?.ToString(), HttpContext.TraceIdentifier, ct);
        return Ok(mappingId);
    }

    [HttpPost("items/unmap")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Unmap([FromBody] UnmapAccountingItemRequest request, CancellationToken ct)
    {
        await mappings.UnmapAsync(request.BrandId, request.LocalItemId, ct);
        var actor = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : (Guid?)null;
        await audit.LogAsync("AccountingItemUnmapped", "AccountingItemMapping", request.LocalItemId.ToString(),
            null, actor, HttpContext.Connection.RemoteIpAddress?.ToString(), HttpContext.TraceIdentifier, ct);
        return NoContent();
    }

    [HttpPost("sync/items")]
    [HttpPost("sync/representatives")]
    [HttpPost("sync/invoices")]
    [ProducesResponseType<long>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> StartSync([FromBody] StartAccountingSyncRequest request, CancellationToken ct)
    {
        var type = HttpContext.Request.Path.Value?.Split('/').Last() switch
        {
            "items" => AccountingSyncType.Items,
            "representatives" => AccountingSyncType.Representatives,
            "invoices" => AccountingSyncType.Invoices,
            _ => throw new InvalidOperationException("Unknown accounting sync type.")
        };
        return Ok(await sync.SyncAsync(request.BrandId, type, ct));
    }

    [HttpPost("sync/history")]
    [ProducesResponseType<IReadOnlyList<AccountingSyncRunView>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SyncHistory([FromBody] AccountingSyncHistoryRequest request, CancellationToken ct)
    {
        if (request.BrandId <= 0 || request.Page is < 1 or > 1000000 || request.PageSize is < 1 or > 100 ||
            (request.Type is not null && !Enum.IsDefined(request.Type.Value)))
            return BadRequest();
        return Ok(await syncRuns.SearchAsync(request.BrandId, request.Type, request.Page, request.PageSize, ct));
    }
}
