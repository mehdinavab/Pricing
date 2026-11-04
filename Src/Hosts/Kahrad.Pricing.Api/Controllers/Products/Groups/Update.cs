using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.ProductGroups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Products.Groups;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class Update(
    IProductGroupService productGroupService,
    IAuditService auditService)
    : ProductGroupsBaseController(productGroupService, auditService)
{
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGroup(
        [FromRoute] long id,
        [FromBody] UpdateProductGroupRequest request,
        CancellationToken cancellationToken)
    {
        await ProductGroupService.UpdateAsync(id, request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "ProductGroupUpdated",
            id.ToString(),
            $"Product group '{request.Name}' with code '{request.Code}' was updated.",
            cancellationToken);

        return NoContent();
    }
}
