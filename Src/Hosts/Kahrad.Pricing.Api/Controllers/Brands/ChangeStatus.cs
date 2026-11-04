using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Brands.Application.DTOs.Brands;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Brands;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class ChangeStatus(
    IBrandService brandService,
    IAuditService auditService)
    : BrandsBaseController(brandService, auditService)
{
    [HttpPatch("{id:long}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeBrandStatus(
        long id,
        [FromBody] ChangeBrandStatusRequest request,
        CancellationToken cancellationToken)
    {
        await BrandService.ChangeStatusAsync(id, request.IsActive, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "BrandStatusChanged",
            id.ToString(),
            $"Brand active status changed to '{request.IsActive}'.",
            cancellationToken);

        return NoContent();
    }
}
