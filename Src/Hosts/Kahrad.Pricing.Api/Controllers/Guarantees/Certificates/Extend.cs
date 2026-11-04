using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Certificates;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Guarantees.Certificates;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
public sealed class Extend(
    IGuaranteeCertificateService certificateService,
    IAuditService auditService)
    : CertificatesBaseController(certificateService, auditService)
{
    [HttpPatch("{id:long}/extend")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExtendCertificate(
        long id,
        [FromBody] ExtendCertificateRequest request,
        CancellationToken cancellationToken)
    {
        await CertificateService.ExtendCertificateAsync(id, request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "CertificateExtended",
            id.ToString(),
            $"Certificate '{id}' was extended by {request.AdditionalMonths} months.",
            cancellationToken);

        return NoContent();
    }
}
