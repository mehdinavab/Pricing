using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Products.Products;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class ChangeStatus(
    IProductService productService,
    IAuditService auditService)
    : ProductsBaseController(productService, auditService)
{
    [HttpPatch("{id:long}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeProductStatus(
        [FromRoute] long id,
        [FromQuery] bool isActive,
        CancellationToken cancellationToken)
    {
        await ProductService.ChangeStatusAsync(id, isActive, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "ProductStatusChanged",
            id.ToString(),
            $"Product '{id}' status was set to {(isActive ? "Active" : "Inactive")}.",
            cancellationToken);

        return NoContent();
    }
}
