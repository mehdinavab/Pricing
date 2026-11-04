using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Sms;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class PagedList(
    ISmsService smsService,
    IAuditService auditService)
    : SmsBaseController(smsService, auditService)
{
    [HttpGet("paged")]
    [ProducesResponseType<BaseResponsePaginated<SmsMessageDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponsePaginated<SmsMessageDto>>> GetPagedMessages(
        [FromQuery] SmsQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await SmsService.GetPagedAsync(query, cancellationToken);
        return Ok(result);
    }
}
