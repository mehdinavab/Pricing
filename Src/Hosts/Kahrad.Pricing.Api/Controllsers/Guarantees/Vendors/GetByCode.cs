using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Vendors;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Guarantees.Vendors;

public sealed class GetByCode(
    IVendorService vendorService,
    IAuditService auditService)
    : VendorsBaseController(vendorService, auditService)
{
    [HttpGet("by-code/{code}")]
    [ProducesResponseType<VendorDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorDto>> GetVendorByCode(
        string code,
        CancellationToken cancellationToken)
    {
        var vendor = await VendorService.GetByCodeAsync(code, cancellationToken);
        return vendor is null ? NotFound() : Ok(vendor);
    }
}
