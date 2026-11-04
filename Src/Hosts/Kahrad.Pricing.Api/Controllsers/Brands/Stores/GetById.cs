using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Brands.Application.DTOs.Stores;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Brands.Stores;

public sealed class GetById(
    IStoreService storeService,
    IAuditService auditService)
    : StoresBaseController(storeService, auditService)
{
    [HttpGet("{id:long}")]
    [ProducesResponseType<StoreDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StoreDto>> GetStore(
        long id,
        CancellationToken cancellationToken)
    {
        var store = await StoreService.GetByIdAsync(id, cancellationToken);
        return store is null ? NotFound() : Ok(store);
    }

    [HttpGet("by-brand/{brandId:long}/code/{code}")]
    [ProducesResponseType<StoreDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StoreDto>> GetStoreByCode(
        long brandId,
        string code,
        CancellationToken cancellationToken)
    {
        var store = await StoreService.GetByCodeAsync(brandId, code, cancellationToken);
        return store is null ? NotFound() : Ok(store);
    }
}
