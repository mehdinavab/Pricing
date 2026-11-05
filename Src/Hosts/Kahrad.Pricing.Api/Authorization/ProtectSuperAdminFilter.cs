using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace Kahrad.Pricing.Api.Authorization;

/// <summary>Only a SuperAdmin may mutate an existing SuperAdmin account.</summary>
public sealed class ProtectSuperAdminFilter(IUserService users) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.User.IsInRole(RoleConstants.SuperAdmin) &&
            context.ActionArguments.TryGetValue("userId", out var value) && value is Guid userId)
        {
            var roles = await users.GetUserRolesAsync(userId, context.HttpContext.RequestAborted);
            if (roles.Contains(RoleConstants.SuperAdmin, StringComparer.OrdinalIgnoreCase))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
        await next();
    }
}
