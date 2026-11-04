using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.ProductGroups;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Products.Groups;

public sealed class GetByCode(
    IProductGroupService productGroupService,
    IAuditService auditService)
    : ProductGroupsBaseController(productGroupService, auditService)
{
    [HttpGet("by-code/{code}")]
    [ProducesResponseType<ProductGroupDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductGroupDto>> GetGroupByCode(
        [FromRoute] string code,
        CancellationToken cancellationToken)
    {
        var group = await ProductGroupService.GetByCodeAsync(code, cancellationToken);
        if (group is null)
            return NotFound();

        return Ok(group);
    }
}
