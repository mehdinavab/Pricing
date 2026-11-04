using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.ProductGroups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Products.Groups;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class Create(
    IProductGroupService productGroupService,
    IAuditService auditService)
    : ProductGroupsBaseController(productGroupService, auditService)
{
    [HttpPost]
    [ProducesResponseType<long>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<long>> CreateGroup(
        [FromBody] CreateProductGroupRequest request,
        CancellationToken cancellationToken)
    {
        var groupId = await ProductGroupService.CreateAsync(request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "ProductGroupCreated",
            groupId.ToString(),
            $"Product group '{request.Name}' with code '{request.Code}' was created.",
            cancellationToken);

        return Created($"/api/product-groups/{groupId}", groupId);
    }
}
