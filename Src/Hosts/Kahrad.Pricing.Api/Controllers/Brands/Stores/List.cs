using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Brands.Application.DTOs.Stores;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Brands.Stores;

public sealed class List(
    IStoreService storeService,
    IAuditService auditService)
    : StoresBaseController(storeService, auditService)
{
    [HttpGet("by-brand/{brandId:long}")]
    [ProducesResponseType<IReadOnlyCollection<StoreDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<StoreDto>>> GetStoresByBrand(
        long brandId,
        CancellationToken cancellationToken)
    {
        return Ok(await StoreService.GetByBrandIdAsync(brandId, cancellationToken));
    }
}
