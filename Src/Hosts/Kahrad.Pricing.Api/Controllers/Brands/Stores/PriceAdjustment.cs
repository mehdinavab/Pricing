using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Brands.Application.DTOs.Stores;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Brands.Stores;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class PriceAdjustment(
    IStoreService storeService,
    IAuditService auditService)
    : StoresBaseController(storeService, auditService)
{
    [HttpPatch("{id:long}/price-adjustment")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePriceAdjustment(
        long id,
        [FromBody] UpdateStorePriceAdjustmentRequest request,
        CancellationToken cancellationToken)
    {
        await StoreService.UpdatePriceAdjustmentAsync(id, request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "StorePriceAdjustmentUpdated",
            id.ToString(),
            $"Store price adjustment updated to {request.Type} with {request.Percentage}%.",
            cancellationToken);

        return NoContent();
    }
}
