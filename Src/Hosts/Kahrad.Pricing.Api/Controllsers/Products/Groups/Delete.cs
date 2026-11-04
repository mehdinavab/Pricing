using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Products.Groups;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class Delete(
    IProductGroupService productGroupService,
    IAuditService auditService)
    : ProductGroupsBaseController(productGroupService, auditService)
{
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGroup(
        [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        await ProductGroupService.DeleteAsync(id, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "ProductGroupDeleted",
            id.ToString(),
            $"Product group with ID '{id}' was deleted.",
            cancellationToken);

        return NoContent();
    }
}
