using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Vendors;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Guarantees.Vendors;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class Create(
    IVendorService vendorService,
    IAuditService auditService)
    : VendorsBaseController(vendorService, auditService)
{
    [HttpPost]
    [ProducesResponseType<long>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<long>> CreateVendor(
        [FromBody] CreateVendorRequest request,
        CancellationToken cancellationToken)
    {
        var vendorId = await VendorService.CreateAsync(request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "VendorCreated",
            vendorId.ToString(),
            $"Vendor '{request.Name}' with code '{request.Code}' was created.",
            cancellationToken);

        return Created($"/api/guarantee-vendors/{vendorId}", vendorId);
    }
}
