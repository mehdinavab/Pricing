using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Brands.Application.DTOs.Brands;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Brands;

public sealed class PagedList(
    IBrandService brandService,
    IAuditService auditService)
    : BrandsBaseController(brandService, auditService)
{
    [HttpGet("paged")]
    [ProducesResponseType<BaseResponsePaginated<BrandDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponsePaginated<BrandDto>>> GetPagedBrands(
        [FromQuery] BrandQueryDto filter,
        CancellationToken cancellationToken)
    {
        return Ok(await BrandService.GetPagedAsync(filter, cancellationToken));
    }
}
