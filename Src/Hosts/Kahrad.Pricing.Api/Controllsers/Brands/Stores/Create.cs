using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Brands.Application.DTOs.Stores;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Brands.Stores;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class Create(
    IStoreService storeService,
    IAuditService auditService)
    : StoresBaseController(storeService, auditService)
{
    [HttpPost]
    [ProducesResponseType<long>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<long>> CreateStore(
        [FromBody] CreateStoreRequest request,
        CancellationToken cancellationToken)
    {
        var storeId = await StoreService.CreateAsync(request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "StoreCreated",
            storeId.ToString(),
            $"Store '{request.Name}' with code '{request.Code}' was created for Brand '{request.BrandId}'.",
            cancellationToken);

        return Created($"/api/stores/{storeId}", storeId);
    }
}
