using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Brands.Application.DTOs.Brands;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Brands;

public sealed class GetById(
    IBrandService brandService,
    IAuditService auditService)
    : BrandsBaseController(brandService, auditService)
{
    [HttpGet("{id:long}")]
    [ProducesResponseType<BrandDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BrandDto>> GetBrand(
        long id,
        CancellationToken cancellationToken)
    {
        var brand = await BrandService.GetByIdAsync(id, cancellationToken);
        return brand is null ? NotFound() : Ok(brand);
    }

    [HttpGet("{id:long}/detail")]
    [ProducesResponseType<BrandDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BrandDetailDto>> GetBrandDetail(
        long id,
        CancellationToken cancellationToken)
    {
        var brand = await BrandService.GetDetailByIdAsync(id, cancellationToken);
        return brand is null ? NotFound() : Ok(brand);
    }
}
