using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Products.Products;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class Create(
    IProductService productService,
    IAuditService auditService)
    : ProductsBaseController(productService, auditService)
{
    [HttpPost]
    [ProducesResponseType<long>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<long>> CreateProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var productId = await ProductService.CreateAsync(request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "ProductCreated",
            productId.ToString(),
            $"Product '{request.Name}' with code '{request.Code}' was created.",
            cancellationToken);

        return Created($"/api/products/{productId}", productId);
    }
}
