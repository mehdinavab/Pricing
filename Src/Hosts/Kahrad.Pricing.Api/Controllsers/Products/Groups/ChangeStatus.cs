using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Products.Groups;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class ChangeStatus(
    IProductGroupService productGroupService,
    IAuditService auditService)
    : ProductGroupsBaseController(productGroupService, auditService)
{
    [HttpPatch("{id:long}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeGroupStatus(
        [FromRoute] long id,
        [FromQuery] bool isActive,
        CancellationToken cancellationToken)
    {
        await ProductGroupService.ChangeStatusAsync(id, isActive, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "ProductGroupStatusChanged",
            id.ToString(),
            $"Product group '{id}' status was set to {(isActive ? "Active" : "Inactive")}.",
            cancellationToken);

        return NoContent();
    }
}
