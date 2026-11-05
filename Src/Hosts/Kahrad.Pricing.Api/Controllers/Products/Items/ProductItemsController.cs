using System.Security.Claims;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Products.Application.Abstractions.Persistence;
using Kahrad.Pricing.Products.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Products.Items;

public sealed record CreateLocalItemRequest(string Code, string Name, string UnitCode);
public sealed record LocalItemSearchRequest(string? Search = null, int Page = 1, int PageSize = 20);
public sealed record ProductCompositionRequest(long ProductId, IReadOnlyList<ProductItemInput> Items);
public sealed record ProductCompositionLookupRequest(long ProductId);

[ApiController]
[Route("api/products/items")]
[Authorize(Roles = RoleConstants.SuperAdmin)]
public sealed class ProductItemsController(ItemCatalogService items, IAuditService audit) : ControllerBase
{
    [HttpPost("create")]
    [ProducesResponseType<long>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateLocalItemRequest request, CancellationToken ct)
    {
        var id = await items.CreateAsync(request.Code, request.Name, request.UnitCode, ct);
        await Audit("LocalItemCreated", id.ToString(), ct);
        return Ok(id);
    }

    [HttpPost("search")]
    [ProducesResponseType<IReadOnlyList<LocalItemView>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromBody] LocalItemSearchRequest request, CancellationToken ct) =>
        Ok(await items.SearchAsync(request.Search, request.Page, request.PageSize, ct));

    [HttpPost("composition/get")]
    [ProducesResponseType<IReadOnlyList<ProductItemView>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetComposition([FromBody] ProductCompositionLookupRequest request, CancellationToken ct) =>
        Ok(await items.GetCompositionAsync(request.ProductId, ct));

    [HttpPost("composition/replace")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ReplaceComposition([FromBody] ProductCompositionRequest request, CancellationToken ct)
    {
        await items.ReplaceCompositionAsync(request.ProductId, request.Items, ct);
        await Audit("ProductCompositionReplaced", request.ProductId.ToString(), ct);
        return NoContent();
    }

    private Task Audit(string action, string id, CancellationToken ct)
    {
        var actor = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : (Guid?)null;
        return audit.LogAsync(action, "ProductItem", id, null, actor,
            HttpContext.Connection.RemoteIpAddress?.ToString(), HttpContext.TraceIdentifier, ct);
    }
}
