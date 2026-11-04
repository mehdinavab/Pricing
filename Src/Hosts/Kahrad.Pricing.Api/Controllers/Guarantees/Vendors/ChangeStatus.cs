using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Vendors;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Guarantees.Vendors;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class ChangeStatus(
    IVendorService vendorService,
    IAuditService auditService)
    : VendorsBaseController(vendorService, auditService)
{
    [HttpPatch("{id:long}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeVendorStatus(
        long id,
        [FromBody] ChangeVendorStatusRequest request,
        CancellationToken cancellationToken)
    {
        await VendorService.ChangeStatusAsync(id, request.IsActive, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "VendorStatusChanged",
            id.ToString(),
            $"Vendor active status changed to '{request.IsActive}'.",
            cancellationToken);

        return NoContent();
    }
}
