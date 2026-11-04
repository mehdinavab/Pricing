using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.ProductGroups;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Products.Groups;

public sealed class GetById(
    IProductGroupService productGroupService,
    IAuditService auditService)
    : ProductGroupsBaseController(productGroupService, auditService)
{
    [HttpGet("{id:long}")]
    [ProducesResponseType<ProductGroupDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductGroupDto>> GetGroupById(
        [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var group = await ProductGroupService.GetByIdAsync(id, cancellationToken);
        if (group is null)
            return NotFound();

        return Ok(group);
    }
}
