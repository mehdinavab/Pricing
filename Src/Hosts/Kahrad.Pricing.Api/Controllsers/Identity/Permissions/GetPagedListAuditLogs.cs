using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Auditing;
using Kahrad.Pricing.Identity.Application.DTOs.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Identity.Permissions
{
    public sealed class GetPagedListAuditLogs(
        IPermissionService permissionService,
        IAuditService auditService)
        : PermissionsBaseController(permissionService, auditService)
    {
        [HttpGet("audit-logs")]
        [ProducesResponseType<BaseResponsePaginated<AuditLogDto>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<BaseResponsePaginated<AuditLogDto>>> GetAuditLogs(
            [FromQuery] AuditLogQueryDto filter,
            CancellationToken cancellationToken)
        {
            return Ok(await AuditService.GetPagedAsync(filter, cancellationToken));
        }
    }
}
