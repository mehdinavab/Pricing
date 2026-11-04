using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Products.Products;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class SetPrimaryImage(
    IProductService productService,
    IAuditService auditService)
    : ProductsBaseController(productService, auditService)
{
    [HttpPost("{productId:long}/gallery/{imageId:long}/primary")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetProductPrimaryImage(
        [FromRoute] long productId,
        [FromRoute] long imageId,
        CancellationToken cancellationToken)
    {
        await ProductService.SetPrimaryImageAsync(productId, imageId, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "ProductPrimaryImageChanged",
            imageId.ToString(),
            $"Gallery image '{imageId}' was set as primary for product '{productId}'.",
            cancellationToken);

        return NoContent();
    }
}
