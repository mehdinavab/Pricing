using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Guarantees.Vendors;

[Authorize(Roles = RoleConstants.SuperAdmin)]
public sealed class Delete(
    IVendorService vendorService,
    IAuditService auditService)
    : VendorsBaseController(vendorService, auditService)
{
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVendor(
        long id,
        CancellationToken cancellationToken)
    {
        await VendorService.DeleteAsync(id, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "VendorDeleted",
            id.ToString(),
            $"Vendor with ID '{id}' was deleted.",
            cancellationToken);

        return NoContent();
    }
}
