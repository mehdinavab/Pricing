using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Vendors;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Guarantees.Vendors;

public sealed class List(
    IVendorService vendorService,
    IAuditService auditService)
    : VendorsBaseController(vendorService, auditService)
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<VendorDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<VendorDto>>> GetAllVendors(
        CancellationToken cancellationToken)
    {
        return Ok(await VendorService.GetAllAsync(cancellationToken));
    }
}
