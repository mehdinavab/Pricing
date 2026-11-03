using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Auditing;
using Kahrad.Pricing.Identity.Application.DTOs.Permissions;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Identity.Permissions
{
    public sealed class GetAuditLogDetails : PermissionsBaseController
    {
        public GetAuditLogDetails(
            IPermissionService permissionService,
            IAuditService auditService)
            : base(permissionService, auditService)
        {
        }

        [HttpGet("audit-logs/{id:long}")]
        [ProducesResponseType<AuditLogDto>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuditLogDto>> GetAuditLog(
            long id,
            CancellationToken cancellationToken)
        {
            var auditLog = await AuditService.GetByIdAsync(id, cancellationToken);
            return auditLog is null ? NotFound() : Ok(auditLog);
        }
    }
}
