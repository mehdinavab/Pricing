using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Brands.Application.DTOs.Brands;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Brands;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class SetActivePriceVersion(
    IBrandService brandService,
    IAuditService auditService)
    : BrandsBaseController(brandService, auditService)
{
    [HttpPatch("{id:long}/active-price-version")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetVersion(
        long id,
        [FromBody] SetActivePriceVersionRequest request,
        CancellationToken cancellationToken)
    {
        await BrandService.SetActivePriceVersionAsync(id, request.Version, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "BrandActivePriceVersionChanged",
            id.ToString(),
            $"Brand active price version changed to '{request.Version}'.",
            cancellationToken);

        return NoContent();
    }
}
