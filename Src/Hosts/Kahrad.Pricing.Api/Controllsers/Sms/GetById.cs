using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Sms;

public sealed class GetById(
    ISmsService smsService,
    IAuditService auditService)
    : SmsBaseController(smsService, auditService)
{
    [HttpGet("{id:long}")]
    [ProducesResponseType<SmsMessageDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SmsMessageDto>> GetMessageById(
        [FromRoute] long id,
        CancellationToken cancellationToken)
    {
        var message = await SmsService.GetByIdAsync(id, cancellationToken);
        if (message is null)
            return NotFound();

        return Ok(message);
    }
}
