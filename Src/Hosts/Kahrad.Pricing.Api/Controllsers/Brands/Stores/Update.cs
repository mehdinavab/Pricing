using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Brands.Application.DTOs.Stores;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Brands.Stores;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class Update(
    IStoreService storeService,
    IAuditService auditService)
    : StoresBaseController(storeService, auditService)
{
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStore(
        long id,
        [FromBody] UpdateStoreRequest request,
        CancellationToken cancellationToken)
    {
        await StoreService.UpdateAsync(id, request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "StoreUpdated",
            id.ToString(),
            $"Store '{request.Name}' with code '{request.Code}' was updated.",
            cancellationToken);

        return NoContent();
    }
}
