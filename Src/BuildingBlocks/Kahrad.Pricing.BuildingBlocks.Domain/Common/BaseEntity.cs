using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kahrad.Pricing.BuildingBlocks.Domain.Common
{
    public class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; protected set; }
        [Required]
        public Guid PublicId { get; private set; } = Guid.NewGuid();
        [Timestamp]
        public byte[] RowVersion { get; private set; } = [];

        public bool IsDeleted { get; private set; }
        [Required]
        public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

        public Guid? CreatedById { get; private set; }

        public DateTime? UpdatedAtUtc { get; private set; }

        public Guid? UpdatedById { get; private set; }

        public DateTime? DeletedAtUtc { get; private set; }

        public Guid? DeletedById { get; private set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public void SetUpdated(Guid? userId, DateTime utcNow)
        {
            UpdatedById = userId;
            UpdatedAtUtc = utcNow;
        }

        public void SetDeleted(Guid? userId, DateTime utcNow)
        {
            IsDeleted = true;
            DeletedById = userId;
            DeletedAtUtc = utcNow;
        }

        public void SetRestored()
        {
            IsDeleted = false;
            DeletedById = null;
            DeletedAtUtc = null;
        }
        public void SetCreated(Guid? userId, DateTime utcNow)
        {
            CreatedById = userId;
            CreatedAtUtc = utcNow;
        }
    }
}