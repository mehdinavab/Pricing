using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Brands.Application.DTOs.Brands;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Brands;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class Update(
    IBrandService brandService,
    IAuditService auditService)
    : BrandsBaseController(brandService, auditService)
{
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBrand(
        long id,
        [FromBody] UpdateBrandRequest request,
        CancellationToken cancellationToken)
    {
        await BrandService.UpdateAsync(id, request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "BrandUpdated",
            id.ToString(),
            $"Brand '{request.Name}' with code '{request.Code}' was updated.",
            cancellationToken);

        return NoContent();
    }
}
