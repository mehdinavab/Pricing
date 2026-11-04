using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Products.Products;

public sealed class List(
    IProductService productService,
    IAuditService auditService)
    : ProductsBaseController(productService, auditService)
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<ProductDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ProductDto>>> GetAllProducts(
        [FromQuery] long? groupId = null,
        [FromQuery] long? brandId = null,
        [FromQuery] bool onlyActive = false,
        CancellationToken cancellationToken = default)
    {
        var products = await ProductService.GetAllAsync(groupId, brandId, onlyActive, cancellationToken);
        return Ok(products);
    }
}
