using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Certificates;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Guarantees.Certificates;

[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin + "," + RoleConstants.Contributor)]
public sealed class Issue(
    IGuaranteeCertificateService certificateService,
    IAuditService auditService)
    : CertificatesBaseController(certificateService, auditService)
{
    [HttpPost]
    [ProducesResponseType<GuaranteeCertificateDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuaranteeCertificateDto>> IssueCertificate(
        [FromBody] IssueCertificateRequest request,
        CancellationToken cancellationToken)
    {
        var certificate = await CertificateService.IssueCertificateAsync(request, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "CertificateIssued",
            certificate.Id.ToString(),
            $"Certificate '{certificate.CertificateNumber}' was issued for Product '{request.ProductId}' with Guarantee '{request.GuaranteeId}'.",
            cancellationToken);

        return Created($"/api/guarantee-certificates/{certificate.Id}", certificate);
    }
}
