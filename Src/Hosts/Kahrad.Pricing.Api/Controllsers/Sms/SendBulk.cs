using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Sms;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class SendBulk(
    ISmsService smsService,
    IAuditService auditService)
    : SmsBaseController(smsService, auditService)
{
    [HttpPost("send-bulk")]
    [ProducesResponseType<SmsBatchResponse>(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SmsBatchResponse>> SendBulkSms(
        [FromBody] SendBulkSmsRequest request,
        CancellationToken cancellationToken)
    {
        var response = await SmsService.SendBulkAsync(request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "BulkSmsQueued",
            response.BatchId.ToString(),
            $"Bulk SMS batch queued for {response.TotalQueued} recipients, scheduled for {response.ScheduledSendDateUtc:u}.",
            cancellationToken);

        return Accepted(response);
    }
}
