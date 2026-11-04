using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Guarantees;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Guarantees.Plans;

public sealed class PagedList(
    IGuaranteeService guaranteeService,
    IAuditService auditService)
    : GuaranteesBaseController(guaranteeService, auditService)
{
    [HttpGet("paged")]
    [ProducesResponseType<BaseResponsePaginated<GuaranteeDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponsePaginated<GuaranteeDto>>> GetPagedGuarantees(
        [FromQuery] GuaranteeQueryDto filter,
        CancellationToken cancellationToken)
    {
        return Ok(await GuaranteeService.GetPagedAsync(filter, cancellationToken));
    }
}
