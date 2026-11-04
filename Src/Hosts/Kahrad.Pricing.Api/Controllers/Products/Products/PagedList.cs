using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Products.Products;

public sealed class PagedList(
    IProductService productService,
    IAuditService auditService)
    : ProductsBaseController(productService, auditService)
{
    [HttpGet("paged")]
    [ProducesResponseType<BaseResponsePaginated<ProductDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponsePaginated<ProductDto>>> GetPagedProducts(
        [FromQuery] ProductQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await ProductService.GetPagedAsync(query, cancellationToken);
        return Ok(result);
    }
}
