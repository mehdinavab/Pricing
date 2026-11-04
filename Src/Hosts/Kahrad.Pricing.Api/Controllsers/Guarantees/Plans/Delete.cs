using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api.Controllsers.Guarantees.Plans;

[Authorize(Roles = RoleConstants.SuperAdmin)]
public sealed class Delete(
    IGuaranteeService guaranteeService,
    IAuditService auditService)
    : GuaranteesBaseController(guaranteeService, auditService)
{
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGuarantee(
        long id,
        CancellationToken cancellationToken)
    {
        await GuaranteeService.DeleteAsync(id, CurrentUserId, cancellationToken);

        await LogAuditAsync(
            "GuaranteeDeleted",
            id.ToString(),
            $"Guarantee with ID '{id}' was deleted.",
            cancellationToken);

        return NoContent();
    }
}
