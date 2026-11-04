using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Guarantees;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Guarantees.Plans;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class Create(
    IGuaranteeService guaranteeService,
    IAuditService auditService)
    : GuaranteesBaseController(guaranteeService, auditService)
{
    [HttpPost]
    [ProducesResponseType<long>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<long>> CreateGuarantee(
        [FromBody] CreateGuaranteeRequest request,
        CancellationToken cancellationToken)
    {
        var guaranteeId = await GuaranteeService.CreateAsync(request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "GuaranteeCreated",
            guaranteeId.ToString(),
            $"Guarantee '{request.Name}' with code '{request.Code}' was created for Vendor '{request.VendorId}'.",
            cancellationToken);

        return Created($"/api/guarantees/{guaranteeId}", guaranteeId);
    }
}
