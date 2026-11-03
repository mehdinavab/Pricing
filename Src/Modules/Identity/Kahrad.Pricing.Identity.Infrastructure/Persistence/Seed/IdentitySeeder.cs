using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kahrad.Pricing.Identity.Infrastructure.Persistence.Seed
{

    public sealed class IdentitySeeder(
        RoleManager<ApplicationRole> roleManager)
    {
        public async Task SeedRolesAsync(
            CancellationToken cancellationToken = default)
        {
            foreach (var roleName in RoleConstants.All)
            {
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
    }

}