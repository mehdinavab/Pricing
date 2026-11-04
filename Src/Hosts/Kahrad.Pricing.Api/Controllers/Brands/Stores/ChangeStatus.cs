using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Brands.Application.DTOs.Stores;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Brands.Stores;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class ChangeStatus(
    IStoreService storeService,
    IAuditService auditService)
    : StoresBaseController(storeService, auditService)
{
    [HttpPatch("{id:long}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeStoreStatus(
        long id,
        [FromBody] ChangeStoreStatusRequest request,
        CancellationToken cancellationToken)
    {
        await StoreService.ChangeStatusAsync(id, request.IsActive, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "StoreStatusChanged",
            id.ToString(),
            $"Store active status changed to '{request.IsActive}'.",
            cancellationToken);

        return NoContent();
    }
}
