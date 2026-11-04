using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Brands.Stores;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class Delete(
    IStoreService storeService,
    IAuditService auditService)
    : StoresBaseController(storeService, auditService)
{
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStore(
        long id,
        CancellationToken cancellationToken)
    {
        await StoreService.DeleteAsync(id, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "StoreDeleted",
            id.ToString(),
            $"Store with ID '{id}' was deleted.",
            cancellationToken);

        return NoContent();
    }
}
