using System.Security.Claims;
using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Representatives.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Accounting;

public sealed record AccountingRepresentativeSearchRequest(long BrandId, string? Search = null, int Page = 1, int PageSize = 20);
public sealed record ImportAccountingRepresentativeRequest(long BrandId, long AccountingRepresentativeId, string LocalCode);
public sealed record MatchAccountingRepresentativeRequest(long BrandId, long RepresentativeId, long AccountingRepresentativeId);

[ApiController]
[Route("api/representatives/accounting")]
[Authorize(Roles = RoleConstants.SuperAdmin)]
public sealed class RepresentativeAccountingController(IAccountingRepresentativeReader external,
    RepresentativeImportService imports, IAuditService audit) : ControllerBase
{
    [HttpPost("search")]
    [ProducesResponseType<IReadOnlyList<ExternalRepresentativeReference>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromBody] AccountingRepresentativeSearchRequest request, CancellationToken ct)
    {
        if (request.BrandId <= 0 || request.Page is < 1 or > 1000000 || request.PageSize is < 1 or > 100)
            return BadRequest();
        return Ok(await external.SearchAsync(request.BrandId, request.Search?.Trim(), request.Page, request.PageSize, ct));
    }

    [HttpPost("import")]
    [ProducesResponseType<long>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Import([FromBody] ImportAccountingRepresentativeRequest request, CancellationToken ct)
    {
        var actor = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : (Guid?)null;
        var representativeId = await imports.ImportAsync(request.BrandId, request.AccountingRepresentativeId,
            request.LocalCode, actor, ct);
        await audit.LogAsync("AccountingRepresentativeImported", "Representative", representativeId.ToString(),
            null, actor, HttpContext.Connection.RemoteIpAddress?.ToString(), HttpContext.TraceIdentifier, ct);
        return Ok(representativeId);
    }

    [HttpPost("map")]
    [ProducesResponseType<long>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Map([FromBody] MatchAccountingRepresentativeRequest request, CancellationToken ct)
    {
        var actor = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : (Guid?)null;
        var mappingId = await imports.MatchAsync(request.BrandId, request.RepresentativeId,
            request.AccountingRepresentativeId, actor, ct);
        await audit.LogAsync("AccountingRepresentativeMatched", "RepresentativeAccountingMapping", mappingId.ToString(),
            null, actor, HttpContext.Connection.RemoteIpAddress?.ToString(), HttpContext.TraceIdentifier, ct);
        return Ok(mappingId);
    }
}
