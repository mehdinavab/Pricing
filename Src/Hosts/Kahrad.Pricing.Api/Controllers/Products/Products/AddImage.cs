using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.Gallery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Products.Products;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class AddImage(
    IProductService productService,
    IAuditService auditService)
    : ProductsBaseController(productService, auditService)
{
    [HttpPost("{productId:long}/gallery")]
    [ProducesResponseType<long>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<long>> AddGalleryImage(
        [FromRoute] long productId,
        [FromBody] AddProductImageRequest request,
        CancellationToken cancellationToken)
    {
        var imageId = await ProductService.AddImageAsync(productId, request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "ProductGalleryImageAdded",
            imageId.ToString(),
            $"Image added to product '{productId}' gallery (IsPrimary={request.IsPrimary}).",
            cancellationToken);

        return Created($"/api/products/{productId}/gallery", imageId);
    }
}
