using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Vendors;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Guarantees.Vendors;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class Update(
    IVendorService vendorService,
    IAuditService auditService)
    : VendorsBaseController(vendorService, auditService)
{
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVendor(
        long id,
        [FromBody] UpdateVendorRequest request,
        CancellationToken cancellationToken)
    {
        await VendorService.UpdateAsync(id, request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "VendorUpdated",
            id.ToString(),
            $"Vendor '{request.Name}' with code '{request.Code}' was updated.",
            cancellationToken);

        return NoContent();
    }
}
