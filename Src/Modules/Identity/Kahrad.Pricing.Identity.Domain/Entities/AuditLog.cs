using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kahrad.Pricing.Identity.Domain.Entities
{
    [Table("AuditLogs", Schema = "identity")]
    [Index(nameof(PublicId), IsUnique = true)]
    [Index(nameof(CreatedAtUtc))]
    [Index(nameof(EntityType), nameof(EntityPublicId), nameof(CreatedAtUtc))]
    public sealed class AuditLog : BaseEntity
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public string Action { get; private set; } = null!;

        [Sieve(CanFilter = true, CanSort = true)]
        public string? EntityType { get; private set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public Guid? EntityPublicId { get; private set; }

        public string? BeforeData { get; private set; }

        public string? AfterData { get; private set; }

        public string? IpAddress { get; private set; }

        public string? CorrelationId { get; private set; }

        private AuditLog()
        {
        }

        public AuditLog(
            string action,
            Guid? actorUserId,
            DateTime createdAtUtc,
            string? entityType = null,
            Guid? entityPublicId = null,
            string? beforeData = null,
            string? afterData = null,
            string? ipAddress = null,
            string? correlationId = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(action);

            Action = action.Trim();
            EntityType = entityType;
            EntityPublicId = entityPublicId;
            BeforeData = beforeData;
            AfterData = afterData;
            IpAddress = ipAddress;
            CorrelationId = correlationId;

            SetCreated(actorUserId, createdAtUtc);
        }
    }
}
