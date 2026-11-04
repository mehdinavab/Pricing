using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Products.Products;

public sealed class CalculatePrice(
    IProductService productService,
    IAuditService auditService)
    : ProductsBaseController(productService, auditService)
{
    [HttpGet("{id:long}/calculate-price")]
    [ProducesResponseType<ProductPriceCalculationDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductPriceCalculationDto>> CalculateProductPrice(
        [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var calculation = await ProductService.CalculateEstimatedPriceAsync(id, cancellationToken);
        return Ok(calculation);
    }
}
