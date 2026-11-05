using System.Security.Claims;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Pricing.Application.Services;
using Kahrad.Pricing.Pricing.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Accounting;

public sealed record PriceOverrideLookupRequest(long BrandId, long ProductId);
public sealed record SavePriceOverrideRequest(long BrandId, long ProductId, FactorySellingPriceMode Mode,
    decimal? FixedFactorySellingPrice, decimal ManualFactoryPriceAdjustment, byte[]? RowVersion);
public sealed record PriceOverrideView(long BrandId, long ProductId, FactorySellingPriceMode Mode,
    decimal? FixedFactorySellingPrice, decimal ManualFactoryPriceAdjustment, byte[] RowVersion);

[ApiController]
[Route("api/pricing/product-override")]
[Authorize(Roles = RoleConstants.SuperAdmin)]
public sealed class ProductPriceOverrideController(ProductPriceOverrideService overrides, IAuditService audit) : ControllerBase
{
    [HttpPost("get")]
    [ProducesResponseType<PriceOverrideView>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromBody] PriceOverrideLookupRequest request, CancellationToken ct)
    {
        var current = await overrides.GetAsync(request.BrandId, request.ProductId, ct);
        return current is null ? NotFound() : Ok(ToView(current));
    }

    [HttpPost("save")]
    [ProducesResponseType<PriceOverrideView>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Save([FromBody] SavePriceOverrideRequest request, CancellationToken ct)
    {
        var actor = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : (Guid?)null;
        var current = await overrides.SaveAsync(request.BrandId, request.ProductId, request.Mode,
            request.FixedFactorySellingPrice, request.ManualFactoryPriceAdjustment, request.RowVersion, actor, ct);
        await audit.LogAsync("ProductFactoryPriceOverrideSaved", "ProductFactoryPriceOverride", current.Id.ToString(),
            null, actor, HttpContext.Connection.RemoteIpAddress?.ToString(), HttpContext.TraceIdentifier, ct);
        return Ok(ToView(current));
    }

    private static PriceOverrideView ToView(ProductFactoryPriceOverride current) =>
        new(current.BrandId, current.ProductId, current.Mode, current.FixedFactorySellingPrice,
            current.ManualFactoryPriceAdjustment, current.RowVersion);
}
