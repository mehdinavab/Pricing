using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Sms;

[AllowAnonymous]
public sealed class SendOtp(
    ISmsService smsService,
    IAuditService auditService)
    : SmsBaseController(smsService, auditService)
{
    [HttpPost("send-otp")]
    [ProducesResponseType<SmsBatchResponse>(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SmsBatchResponse>> SendOtpSms(
        [FromBody] SendOtpSmsRequest request,
        CancellationToken cancellationToken)
    {
        var response = await SmsService.SendOtpAsync(request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "OtpSmsQueued",
            response.BatchId.ToString(),
            $"OTP SMS queued for '{request.PhoneNumber}'.",
            cancellationToken);

        return Accepted(response);
    }
}
