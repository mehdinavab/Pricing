using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Kahrad.Pricing.Identity.Domain.Entities
{
    [Table("Permissions", Schema = "identity")]
    [Index(nameof(PublicId), IsUnique = true)]
    [Index(nameof(Code), IsUnique = true)]
    public sealed class Permission : BaseEntity
    {
        public string Code { get; private set; } = null!;

        public string Name { get; private set; } = null!;

        private Permission()
        {
        }

        public Permission(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }
}