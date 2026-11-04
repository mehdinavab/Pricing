using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.Gallery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Products.Products;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class UpdateImage(
    IProductService productService,
    IAuditService auditService)
    : ProductsBaseController(productService, auditService)
{
    [HttpPut("gallery/{imageId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGalleryImage(
        [FromRoute] long imageId,
        [FromBody] UpdateProductImageRequest request,
        CancellationToken cancellationToken)
    {
        await ProductService.UpdateImageAsync(imageId, request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "ProductGalleryImageUpdated",
            imageId.ToString(),
            $"Gallery image '{imageId}' was updated.",
            cancellationToken);

        return NoContent();
    }
}
