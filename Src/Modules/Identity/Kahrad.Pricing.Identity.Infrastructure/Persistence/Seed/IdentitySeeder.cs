using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Kahrad.Pricing.Identity.Infrastructure.Persistence.Seed
{

    public sealed class IdentitySeeder(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IdentityDbContext context,
        IHostEnvironment environment)
    {
        public async Task SeedRolesAsync(
            CancellationToken cancellationToken = default)
        {
            foreach (var roleName in RoleConstants.All)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (await roleManager.RoleExistsAsync(roleName))
                    continue;

                var result = await roleManager.CreateAsync(
                    new ApplicationRole(roleName));

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to create role '{roleName}': " +
                        string.Join(", ", result.Errors.Select(x => x.Description)));
                }
            }
        }
        public async Task SeedDevelopmentSuperAdminAsync(
            string username,
            string password,
            CancellationToken cancellationToken = default)
        {
            if (!environment.IsDevelopment())
                throw new InvalidOperationException("The development superadmin seed is only available in Development.");
            ArgumentException.ThrowIfNullOrWhiteSpace(username);
            ArgumentException.ThrowIfNullOrWhiteSpace(password);
            cancellationToken.ThrowIfCancellationRequested();

            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            await SeedRolesAsync(cancellationToken);
            var user = await userManager.FindByNameAsync(username);
            if (user is null)
            {
                user = new ApplicationUser(username, "Mehdi", "Navab");
                // The explicitly requested development bootstrap password may be below normal policy.
                // Hash with Identity; user validation and normal registration password rules remain intact.
                user.PasswordHash = userManager.PasswordHasher.HashPassword(user, password);
                EnsureSucceeded(await userManager.CreateAsync(user));
            }
            else if (!await userManager.CheckPasswordAsync(user, password))
            {
                throw new InvalidOperationException("The seed user already exists with a different password; no password was overwritten.");
            }

            if (!user.IsActive)
            {
                user.Activate();
                EnsureSucceeded(await userManager.UpdateAsync(user));
            }
            if (!await userManager.IsInRoleAsync(user, RoleConstants.SuperAdmin))
                EnsureSucceeded(await userManager.AddToRoleAsync(user, RoleConstants.SuperAdmin));

            if (!await userManager.CheckPasswordAsync(user, password) ||
                !await userManager.IsInRoleAsync(user, RoleConstants.SuperAdmin))
                throw new InvalidOperationException("Superadmin seed verification failed.");
            await transaction.CommitAsync(cancellationToken);
        }

        private static void EnsureSucceeded(IdentityResult result)
        {
            if (!result.Succeeded)
                throw new InvalidOperationException("Identity seed failed: " + string.Join(", ", result.Errors.Select(x => x.Code)));
        }
    }

}