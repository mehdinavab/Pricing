using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Sms;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class ProcessOutbox(
    ISmsService smsService,
    ISmsOutboxProcessor outboxProcessor,
    IAuditService auditService)
    : SmsBaseController(smsService, auditService)
{
    [HttpPost("process-outbox")]
    [ProducesResponseType<SmsOutboxProcessResult>(StatusCodes.Status200OK)]
    public async Task<ActionResult<SmsOutboxProcessResult>> TriggerOutboxProcessing(
        CancellationToken cancellationToken)
    {
        var result = await outboxProcessor.ProcessPendingOutboxAsync(cancellationToken);

        await LogAuditAsync(
            "SmsOutboxProcessed",
            null,
            $"SMS outbox processed manually: Sent={result.SentCount}, Failed={result.FailedCount}, Expired={result.ExpiredCount}.",
            cancellationToken);

        return Ok(result);
    }
}
