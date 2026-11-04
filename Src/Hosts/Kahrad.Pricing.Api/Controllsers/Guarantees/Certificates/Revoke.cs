using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Certificates;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Guarantees.Certificates;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class Revoke(
    IGuaranteeCertificateService certificateService,
    IAuditService auditService)
    : CertificatesBaseController(certificateService, auditService)
{
    [HttpPatch("{id:long}/revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeCertificate(
        long id,
        [FromBody] RevokeCertificateRequest request,
        CancellationToken cancellationToken)
    {
        await CertificateService.RevokeCertificateAsync(id, request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "CertificateRevoked",
            id.ToString(),
            $"Certificate '{id}' was revoked. Reason: {request.Reason}",
            cancellationToken);

        return NoContent();
    }
}
