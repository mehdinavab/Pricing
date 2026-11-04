using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Brands;

[Authorize(Roles = RoleConstants.SuperAdmin)]
public sealed class Delete(
    IBrandService brandService,
    IAuditService auditService)
    : BrandsBaseController(brandService, auditService)
{
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBrand(
        long id,
        CancellationToken cancellationToken)
    {
        await BrandService.DeleteAsync(id, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "BrandDeleted",
            id.ToString(),
            $"Brand with ID '{id}' was deleted.",
            cancellationToken);

        return NoContent();
    }
}
