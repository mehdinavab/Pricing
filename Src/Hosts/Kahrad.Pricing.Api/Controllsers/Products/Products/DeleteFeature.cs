using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Products.Products;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class DeleteFeature(
    IProductService productService,
    IAuditService auditService)
    : ProductsBaseController(productService, auditService)
{
    [HttpDelete("features/{featureId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProductFeature(
        [FromRoute] long featureId,
        CancellationToken cancellationToken)
    {
        await ProductService.DeleteFeatureAsync(featureId, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "ProductFeatureDeleted",
            featureId.ToString(),
            $"Feature '{featureId}' was deleted.",
            cancellationToken);

        return NoContent();
    }
}
