using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Guarantees;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Guarantees.Plans;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class ChangeStatus(
    IGuaranteeService guaranteeService,
    IAuditService auditService)
    : GuaranteesBaseController(guaranteeService, auditService)
{
    [HttpPatch("{id:long}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeGuaranteeStatus(
        long id,
        [FromBody] ChangeGuaranteeStatusRequest request,
        CancellationToken cancellationToken)
    {
        await GuaranteeService.ChangeStatusAsync(id, request.IsActive, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "GuaranteeStatusChanged",
            id.ToString(),
            $"Guarantee active status changed to '{request.IsActive}'.",
            cancellationToken);

        return NoContent();
    }
}
