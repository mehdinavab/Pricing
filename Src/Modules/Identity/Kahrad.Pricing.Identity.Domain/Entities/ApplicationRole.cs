using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Kahrad.Pricing.Identity.Domain.Entities
{
    [Table("AspNetRoles", Schema = "identity")]
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string? Description { get; private set; }

        private ApplicationRole()
        {
        }

        public ApplicationRole(string roleName, string? description = null) : base(roleName)
        {
            Description = description?.Trim();
        }

        public void UpdateDescription(string? description)
        {
            Description = description?.Trim();
        }
    }
}
