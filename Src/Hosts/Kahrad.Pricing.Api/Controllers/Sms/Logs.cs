using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Sms;

public sealed class Logs(ISmsService smsService, IAuditService auditService) : SmsBaseController(smsService, auditService)
{
    /// <summary>Returns a page of delivery attempts for a queued SMS. Authentication is required.</summary>
    [HttpGet("{id:long}/logs")]
    [ProducesResponseType<IReadOnlyCollection<SmsLogDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<SmsLogDto>>> GetLogs(
        long id, CancellationToken cancellationToken, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(await SmsService.GetLogsAsync(id, page, pageSize, cancellationToken));
    }
}
