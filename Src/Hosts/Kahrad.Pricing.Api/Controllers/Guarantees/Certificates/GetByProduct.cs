using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Certificates;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Guarantees.Certificates;

public sealed class GetByProduct(
    IGuaranteeCertificateService certificateService,
    IAuditService auditService)
    : CertificatesBaseController(certificateService, auditService)
{
    [HttpGet("by-product/{productId:long}")]
    [ProducesResponseType<IReadOnlyCollection<GuaranteeCertificateDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<GuaranteeCertificateDto>>> GetCertificatesByProduct(
        long productId,
        CancellationToken cancellationToken)
    {
        return Ok(await CertificateService.GetByProductIdAsync(productId, cancellationToken));
    }
}
