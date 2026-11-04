using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Vendors;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Guarantees.Vendors;

public sealed class PagedList(
    IVendorService vendorService,
    IAuditService auditService)
    : VendorsBaseController(vendorService, auditService)
{
    [HttpGet("paged")]
    [ProducesResponseType<BaseResponsePaginated<VendorDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponsePaginated<VendorDto>>> GetPagedVendors(
        [FromQuery] VendorQueryDto filter,
        CancellationToken cancellationToken)
    {
        return Ok(await VendorService.GetPagedAsync(filter, cancellationToken));
    }
}
