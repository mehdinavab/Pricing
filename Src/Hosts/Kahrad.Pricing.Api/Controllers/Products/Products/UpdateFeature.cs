using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Products.Products;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class UpdateFeature(
    IProductService productService,
    IAuditService auditService)
    : ProductsBaseController(productService, auditService)
{
    [HttpPut("features/{featureId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProductFeature(
        [FromRoute] long featureId,
        [FromBody] UpdateProductFeatureRequest request,
        CancellationToken cancellationToken)
    {
        await ProductService.UpdateFeatureAsync(featureId, request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "ProductFeatureUpdated",
            featureId.ToString(),
            $"Feature '{featureId}' was updated (PriceEffect={request.HasPriceEffect}, %={request.PriceEffectPercentage}, Amount={request.PriceEffectAmount}).",
            cancellationToken);

        return NoContent();
    }
}
