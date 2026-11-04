using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Products.Products;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class UpdatePricingFactors(
    IProductService productService,
    IAuditService auditService)
    : ProductsBaseController(productService, auditService)
{
    [HttpPatch("{id:long}/pricing-factors")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateFactors(
        [FromRoute] long id,
        [FromBody] UpdateProductPricingFactorsRequest request,
        CancellationToken cancellationToken)
    {
        await ProductService.UpdatePricingFactorsAsync(id, request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "ProductPricingFactorsUpdated",
            id.ToString(),
            $"Updated pricing factors for product '{id}': Beauty={request.BeautyFactorPercentage}%, Commercial={request.CommercialFactorPercentage}%, Sales={request.SalesFactorPercentage}%.",
            cancellationToken);

        return NoContent();
    }
}
