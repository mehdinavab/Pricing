using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Brands.Application.DTOs.Stores;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Brands.Stores;

public sealed class PagedList(
    IStoreService storeService,
    IAuditService auditService)
    : StoresBaseController(storeService, auditService)
{
    [HttpGet("paged")]
    [ProducesResponseType<BaseResponsePaginated<StoreDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponsePaginated<StoreDto>>> GetPagedStores(
        [FromQuery] StoreQueryDto filter,
        CancellationToken cancellationToken)
    {
        return Ok(await StoreService.GetPagedAsync(filter, cancellationToken));
    }
}
