using Kahrad.Pricing.Sms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kahrad.Pricing.Sms.Infrastructure.Persistence.Config;

public sealed class SmsProviderConfigurationMapping : IEntityTypeConfiguration<SmsProviderConfiguration>
{
    public void Configure(EntityTypeBuilder<SmsProviderConfiguration> builder)
    {
        builder.ToTable("ProviderConfigurations", "sms");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.PublicId).IsUnique();
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Provider).HasMaxLength(30).IsRequired();
        builder.Property(x => x.ApiKeyConfigurationKey).HasMaxLength(200).IsRequired();
        builder.Property(x => x.LineNumber).HasMaxLength(30);
        builder.Property(x => x.TemplateMappingsJson).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class SmsModuleConfigurationMapping : IEntityTypeConfiguration<SmsModuleConfiguration>
{
    public void Configure(EntityTypeBuilder<SmsModuleConfiguration> builder)
    {
        builder.ToTable("ModuleConfigurations", "sms", table =>
            table.HasCheckConstraint("CK_SmsModule_ModuleKey", "[ModuleKey] = N'Sms'"));
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.PublicId).IsUnique();
        builder.Property(x => x.ModuleKey).HasMaxLength(30).IsRequired();
        builder.HasIndex(x => x.ModuleKey).IsUnique();
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasOne<SmsProviderConfiguration>().WithMany().HasForeignKey(x => x.ActiveProviderConfigurationId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public sealed class SmsLogMapping : IEntityTypeConfiguration<SmsLog>
{
    public void Configure(EntityTypeBuilder<SmsLog> builder)
    {
        builder.ToTable("Logs", "sms");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.PublicId).IsUnique();
        builder.HasIndex(x => new { x.SmsMessageId, x.AttemptNumber }).IsUnique();
        builder.HasIndex(x => new { x.Status, x.StartedAtUtc });
        builder.Property(x => x.ProviderMessageId).HasMaxLength(100);
        builder.Property(x => x.ErrorCode).HasMaxLength(100);
        builder.Property(x => x.Cost).HasPrecision(18, 4);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasOne<SmsMessage>().WithMany().HasForeignKey(x => x.SmsMessageId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<SmsProviderConfiguration>().WithMany().HasForeignKey(x => x.ProviderConfigurationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
