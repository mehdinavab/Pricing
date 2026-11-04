using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Guarantees;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Guarantees.Plans;

public sealed class GetById(
    IGuaranteeService guaranteeService,
    IAuditService auditService)
    : GuaranteesBaseController(guaranteeService, auditService)
{
    [HttpGet("{id:long}")]
    [ProducesResponseType<GuaranteeDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuaranteeDto>> GetGuarantee(
        long id,
        CancellationToken cancellationToken)
    {
        var guarantee = await GuaranteeService.GetByIdAsync(id, cancellationToken);
        return guarantee is null ? NotFound() : Ok(guarantee);
    }

    [HttpGet("{id:long}/detail")]
    [ProducesResponseType<GuaranteeDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuaranteeDetailDto>> GetGuaranteeDetail(
        long id,
        CancellationToken cancellationToken)
    {
        var guarantee = await GuaranteeService.GetDetailByIdAsync(id, cancellationToken);
        return guarantee is null ? NotFound() : Ok(guarantee);
    }
}
