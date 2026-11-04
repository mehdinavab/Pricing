using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Brands.Application.DTOs.BrandConfigs;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Brands;

public sealed class Configs(
    IBrandService brandService,
    IAuditService auditService)
    : BrandsBaseController(brandService, auditService)
{
    [HttpGet("{id:long}/configs")]
    [ProducesResponseType<IReadOnlyCollection<BrandConfigDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<BrandConfigDto>>> GetConfigs(
        long id,
        CancellationToken cancellationToken)
    {
        return Ok(await BrandService.GetConfigsAsync(id, cancellationToken));
    }

    [HttpPut("{id:long}/configs")]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetConfigs(
        long id,
        [FromBody] IEnumerable<SetBrandConfigRequest> configs,
        CancellationToken cancellationToken)
    {
        await BrandService.SetConfigsAsync(id, configs, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "BrandConfigsUpdated",
            id.ToString(),
            $"Configs for Brand '{id}' were updated.",
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:long}/configs/{key}")]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteConfig(
        long id,
        string key,
        CancellationToken cancellationToken)
    {
        await BrandService.DeleteConfigAsync(id, key, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "BrandConfigDeleted",
            id.ToString(),
            $"Config key '{key}' for Brand '{id}' was deleted.",
            cancellationToken);

        return NoContent();
    }
}
