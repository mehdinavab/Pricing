using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllers.Identity.Permissions
{
    [Route("api/permissions")]
    [ApiController]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
    public class PermissionsBaseController : ControllerBase
    {
        public PermissionsBaseController(
            IPermissionService permissionService,
            IAuditService auditService)
        {
            PermissionService = permissionService;
            AuditService = auditService;
        }

        protected IPermissionService PermissionService { get; }
        protected IAuditService AuditService { get; }
    }
}
