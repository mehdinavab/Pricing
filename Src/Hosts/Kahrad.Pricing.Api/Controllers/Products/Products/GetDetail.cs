using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Products.Products;

public sealed class GetDetail(
    IProductService productService,
    IAuditService auditService)
    : ProductsBaseController(productService, auditService)
{
    [HttpGet("{id:long}/detail")]
    [ProducesResponseType<ProductDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDetailDto>> GetProductDetail(
        [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var product = await ProductService.GetDetailByIdAsync(id, cancellationToken);
        if (product is null)
            return NotFound();

        return Ok(product);
    }
}
