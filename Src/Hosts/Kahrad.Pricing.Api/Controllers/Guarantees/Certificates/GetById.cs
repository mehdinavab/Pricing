using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Certificates;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Guarantees.Certificates;

public sealed class GetById(
    IGuaranteeCertificateService certificateService,
    IAuditService auditService)
    : CertificatesBaseController(certificateService, auditService)
{
    [HttpGet("{id:long}")]
    [ProducesResponseType<GuaranteeCertificateDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuaranteeCertificateDto>> GetCertificate(
        long id,
        CancellationToken cancellationToken)
    {
        var certificate = await CertificateService.GetByIdAsync(id, cancellationToken);
        return certificate is null ? NotFound() : Ok(certificate);
    }
}
