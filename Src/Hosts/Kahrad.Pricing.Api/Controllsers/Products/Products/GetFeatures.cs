using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.Features;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Products.Products;

public sealed class GetFeatures(
    IProductService productService,
    IAuditService auditService)
    : ProductsBaseController(productService, auditService)
{
    [HttpGet("{productId:long}/features")]
    [ProducesResponseType<IReadOnlyCollection<ProductFeatureDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<ProductFeatureDto>>> GetProductFeatures(
        [FromRoute] long productId,
        [FromQuery] bool? hasPriceEffect = null,
        CancellationToken cancellationToken = default)
    {
        var features = await ProductService.GetFeaturesAsync(productId, hasPriceEffect, cancellationToken);
        return Ok(features);
    }
}
