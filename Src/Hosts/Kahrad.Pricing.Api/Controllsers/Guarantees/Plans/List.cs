using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Guarantees;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Guarantees.Plans;

public sealed class List(
    IGuaranteeService guaranteeService,
    IAuditService auditService)
    : GuaranteesBaseController(guaranteeService, auditService)
{
    [HttpGet("by-vendor/{vendorId:long}")]
    [ProducesResponseType<IReadOnlyCollection<GuaranteeDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<GuaranteeDto>>> GetGuaranteesByVendor(
        long vendorId,
        CancellationToken cancellationToken)
    {
        return Ok(await GuaranteeService.GetByVendorIdAsync(vendorId, cancellationToken));
    }
}
