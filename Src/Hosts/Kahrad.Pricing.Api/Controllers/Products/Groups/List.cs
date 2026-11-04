using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.ProductGroups;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Products.Groups;

public sealed class List(
    IProductGroupService productGroupService,
    IAuditService auditService)
    : ProductGroupsBaseController(productGroupService, auditService)
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<ProductGroupDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ProductGroupDto>>> GetAllGroups(
        [FromQuery] bool onlyActive = false,
        CancellationToken cancellationToken = default)
    {
        var groups = await ProductGroupService.GetAllAsync(onlyActive, cancellationToken);
        return Ok(groups);
    }
}
