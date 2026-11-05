using Kahrad.Pricing.Sms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kahrad.Pricing.Sms.Infrastructure.Persistence.Config;

public sealed class SmsMessageConfiguration : IEntityTypeConfiguration<SmsMessage>
{
    public void Configure(EntityTypeBuilder<SmsMessage> builder)
    {
        builder.ToTable("Messages", "sms");

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.IsDeleted);
        builder.HasOne<SmsProviderConfiguration>().WithMany().HasForeignKey(x => x.ProviderConfigurationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.PublicId)
            .IsRequired();

        builder.HasIndex(x => x.PublicId)
            .IsUnique();

        builder.Property(x => x.BatchId)
            .IsRequired();

        builder.HasIndex(x => x.BatchId);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(x => x.PhoneNumber);

        builder.Property(x => x.MessageBody)
            .HasMaxLength(2000);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.HasIndex(x => x.Type);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.HasIndex(x => x.Status);

        builder.Property(x => x.TemplateId);

        builder.Property(x => x.TemplateParameters)
            .HasMaxLength(4000);

        builder.Property(x => x.LineNumber)
            .HasMaxLength(30);

        builder.Property(x => x.SendDateUtc)
            .IsRequired();

        builder.HasIndex(x => x.SendDateUtc);

        builder.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.ExpiresAtUtc);

        builder.Property(x => x.SentAtUtc);

        builder.Property(x => x.ProviderMessageId)
            .HasMaxLength(100);

        builder.Property(x => x.Cost)
            .HasPrecision(18, 4);

        builder.Property(x => x.RetryCount)
            .HasDefaultValue(0);

        builder.Property(x => x.Provider).HasMaxLength(30).HasDefaultValue("SmsIr");
        builder.Property(x => x.NextAttemptAtUtc);
        builder.HasIndex(x => new { x.Status, x.NextAttemptAtUtc, x.SendDateUtc });

        builder.Property(x => x.FailureReason)
            .HasMaxLength(2000);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
