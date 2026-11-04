using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Vendors;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Guarantees.Vendors;

public sealed class GetById(
    IVendorService vendorService,
    IAuditService auditService)
    : VendorsBaseController(vendorService, auditService)
{
    [HttpGet("{id:long}")]
    [ProducesResponseType<VendorDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorDto>> GetVendor(
        long id,
        CancellationToken cancellationToken)
    {
        var vendor = await VendorService.GetByIdAsync(id, cancellationToken);
        return vendor is null ? NotFound() : Ok(vendor);
    }

    [HttpGet("{id:long}/detail")]
    [ProducesResponseType<VendorDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorDetailDto>> GetVendorDetail(
        long id,
        CancellationToken cancellationToken)
    {
        var vendor = await VendorService.GetDetailByIdAsync(id, cancellationToken);
        return vendor is null ? NotFound() : Ok(vendor);
    }
}
