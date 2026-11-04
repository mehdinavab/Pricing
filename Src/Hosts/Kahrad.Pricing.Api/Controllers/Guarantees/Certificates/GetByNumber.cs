using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Certificates;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Guarantees.Certificates;

public sealed class GetByNumber(
    IGuaranteeCertificateService certificateService,
    IAuditService auditService)
    : CertificatesBaseController(certificateService, auditService)
{
    [HttpGet("by-number/{certificateNumber}")]
    [ProducesResponseType<GuaranteeCertificateDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuaranteeCertificateDto>> GetCertificateByNumber(
        string certificateNumber,
        CancellationToken cancellationToken)
    {
        var certificate = await CertificateService.GetByCertificateNumberAsync(certificateNumber, cancellationToken);
        return certificate is null ? NotFound() : Ok(certificate);
    }
}
