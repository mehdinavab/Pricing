using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.ProductGroups;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Products.Groups;

public sealed class PagedList(
    IProductGroupService productGroupService,
    IAuditService auditService)
    : ProductGroupsBaseController(productGroupService, auditService)
{
    [HttpGet("paged")]
    [ProducesResponseType<BaseResponsePaginated<ProductGroupDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponsePaginated<ProductGroupDto>>> GetPagedGroups(
        [FromQuery] ProductGroupQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await ProductGroupService.GetPagedAsync(query, cancellationToken);
        return Ok(result);
    }
}
