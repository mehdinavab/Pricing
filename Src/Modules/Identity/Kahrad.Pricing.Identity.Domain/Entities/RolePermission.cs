using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kahrad.Pricing.Identity.Domain.Entities
{
    [Table("RolePermissions", Schema = "identity")]
    [Index(nameof(PublicId), IsUnique = true)]
    [Index(nameof(RoleId), nameof(PermissionId), IsUnique = true)]
    [Index(nameof(PermissionId))]
    public sealed class RolePermission : BaseEntity
    {
        public Guid RoleId { get; private set; }

        public long PermissionId { get; private set; }

        private RolePermission()
        {
        }

        public RolePermission(Guid roleId, long permissionId)
        {
            if (roleId == Guid.Empty)
                throw new ArgumentException("RoleId cannot be empty.", nameof(roleId));

            if (permissionId <= 0)
                throw new ArgumentOutOfRangeException(nameof(permissionId));

            RoleId = roleId;
            PermissionId = permissionId;
        }
    }
}
