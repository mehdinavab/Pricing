using FluentValidation;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.Auditing;
using Kahrad.Pricing.Identity.Application.Authentication;
using Kahrad.Pricing.Identity.Application.Permissions;
using Kahrad.Pricing.Identity.Application.Roles;
using Kahrad.Pricing.Identity.Application.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Kahrad.Pricing.Identity.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentityApplication(
            this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddValidatorsFromAssembly(assembly);

            services.AddAutoMapper(cfg => { }, assembly);

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }

}