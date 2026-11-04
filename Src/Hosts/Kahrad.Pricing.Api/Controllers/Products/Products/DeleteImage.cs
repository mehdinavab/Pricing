using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Products.Products;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class DeleteImage(
    IProductService productService,
    IAuditService auditService)
    : ProductsBaseController(productService, auditService)
{
    [HttpDelete("gallery/{imageId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGalleryImage(
        [FromRoute] long imageId,
        CancellationToken cancellationToken)
    {
        await ProductService.DeleteImageAsync(imageId, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "ProductGalleryImageDeleted",
            imageId.ToString(),
            $"Gallery image '{imageId}' was deleted.",
            cancellationToken);

        return NoContent();
    }
}
