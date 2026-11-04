using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Products.Products;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class AddFeature(
    IProductService productService,
    IAuditService auditService)
    : ProductsBaseController(productService, auditService)
{
    [HttpPost("{productId:long}/features")]
    [ProducesResponseType<long>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<long>> AddProductFeature(
        [FromRoute] long productId,
        [FromBody] AddProductFeatureRequest request,
        CancellationToken cancellationToken)
    {
        var featureId = await ProductService.AddFeatureAsync(productId, request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "ProductFeatureAdded",
            featureId.ToString(),
            $"Feature '{request.Name}' added to product '{productId}' (PriceEffect={request.HasPriceEffect}, %={request.PriceEffectPercentage}, Amount={request.PriceEffectAmount}).",
            cancellationToken);

        return Created($"/api/products/{productId}/features", featureId);
    }
}
