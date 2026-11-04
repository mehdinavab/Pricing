using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Sms;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class SendTemplate(
    ISmsService smsService,
    IAuditService auditService)
    : SmsBaseController(smsService, auditService)
{
    [HttpPost("send-template")]
    [ProducesResponseType<SmsBatchResponse>(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SmsBatchResponse>> SendTemplateSms(
        [FromBody] SendTemplateSmsRequest request,
        CancellationToken cancellationToken)
    {
        var response = await SmsService.SendTemplateAsync(request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "TemplateSmsQueued",
            response.BatchId.ToString(),
            $"Template SMS queued for '{request.PhoneNumber}' using template ID '{request.TemplateId}'.",
            cancellationToken);

        return Accepted(response);
    }
}
