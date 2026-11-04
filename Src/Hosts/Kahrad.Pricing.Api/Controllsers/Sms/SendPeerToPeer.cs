using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Sms;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class SendPeerToPeer(
    ISmsService smsService,
    IAuditService auditService)
    : SmsBaseController(smsService, auditService)
{
    [HttpPost("send-peer-to-peer")]
    [ProducesResponseType<SmsBatchResponse>(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SmsBatchResponse>> SendPeerToPeerSms(
        [FromBody] SendPeerToPeerSmsRequest request,
        CancellationToken cancellationToken)
    {
        var response = await SmsService.SendPeerToPeerAsync(request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "PeerToPeerSmsQueued",
            response.BatchId.ToString(),
            $"Peer-to-peer SMS batch queued for {response.TotalQueued} recipients, scheduled for {response.ScheduledSendDateUtc:u}.",
            cancellationToken);

        return Accepted(response);
    }
}
