using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.Gallery;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Products.Products;

public sealed class GetGallery(
    IProductService productService,
    IAuditService auditService)
    : ProductsBaseController(productService, auditService)
{
    [HttpGet("{productId:long}/gallery")]
    [ProducesResponseType<IReadOnlyCollection<ProductImageDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<ProductImageDto>>> GetProductGallery(
        [FromRoute] long productId,
        CancellationToken cancellationToken = default)
    {
        var gallery = await ProductService.GetGalleryAsync(productId, cancellationToken);
        return Ok(gallery);
    }
}
