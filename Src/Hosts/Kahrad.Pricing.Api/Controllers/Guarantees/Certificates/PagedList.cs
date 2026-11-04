using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Certificates;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Guarantees.Certificates;

public sealed class PagedList(
    IGuaranteeCertificateService certificateService,
    IAuditService auditService)
    : CertificatesBaseController(certificateService, auditService)
{
    [HttpGet("paged")]
    [ProducesResponseType<BaseResponsePaginated<GuaranteeCertificateDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponsePaginated<GuaranteeCertificateDto>>> GetPagedCertificates(
        [FromQuery] CertificateQueryDto filter,
        CancellationToken cancellationToken)
    {
        return Ok(await CertificateService.GetPagedAsync(filter, cancellationToken));
    }
}
