using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Brands.Application.DTOs.Brands;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Brands;

public sealed class List(
    IBrandService brandService,
    IAuditService auditService)
    : BrandsBaseController(brandService, auditService)
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<BrandDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<BrandDto>>> GetAllBrands(
        CancellationToken cancellationToken)
    {
        return Ok(await BrandService.GetAllAsync(cancellationToken));
    }
}
