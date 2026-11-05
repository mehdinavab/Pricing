using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kahrad.Pricing.Sms.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSmsLogsAndConfigurationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProviderConfigurationId",
                schema: "sms",
                table: "Messages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ProviderConfigurations",
                schema: "sms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ApiKeyConfigurationKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LineNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TimeoutSeconds = table.Column<int>(type: "int", nullable: false),
                    TemplateMappingsJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                schema: "sms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SmsMessageId = table.Column<long>(type: "bigint", nullable: false),
                    ProviderConfigurationId = table.Column<long>(type: "bigint", nullable: false),
                    AttemptNumber = table.Column<int>(type: "int", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CanRetry = table.Column<bool>(type: "bit", nullable: false),
                    ProviderMessageId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ErrorCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logs_Messages_SmsMessageId",
                        column: x => x.SmsMessageId,
                        principalSchema: "sms",
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Logs_ProviderConfigurations_ProviderConfigurationId",
                        column: x => x.ProviderConfigurationId,
                        principalSchema: "sms",
                        principalTable: "ProviderConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ModuleConfigurations",
                schema: "sms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleKey = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ActiveProviderConfigurationId = table.Column<long>(type: "bigint", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    BatchSize = table.Column<int>(type: "int", nullable: false),
                    RetryEnabled = table.Column<bool>(type: "bit", nullable: false),
                    MaxAttempts = table.Column<int>(type: "int", nullable: false),
                    InitialRetryDelaySeconds = table.Column<int>(type: "int", nullable: false),
                    MaxRetryDelaySeconds = table.Column<int>(type: "int", nullable: false),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleConfigurations", x => x.Id);
                    table.CheckConstraint("CK_SmsModule_ModuleKey", "[ModuleKey] = N'Sms'");
                    table.ForeignKey(
                        name: "FK_ModuleConfigurations_ProviderConfigurations_ActiveProviderConfigurationId",
                        column: x => x.ActiveProviderConfigurationId,
                        principalSchema: "sms",
                        principalTable: "ProviderConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ProviderConfigurationId",
                schema: "sms",
                table: "Messages",
                column: "ProviderConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ProviderConfigurationId",
                schema: "sms",
                table: "Logs",
                column: "ProviderConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_PublicId",
                schema: "sms",
                table: "Logs",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_SmsMessageId_AttemptNumber",
                schema: "sms",
                table: "Logs",
                columns: new[] { "SmsMessageId", "AttemptNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Status_StartedAtUtc",
                schema: "sms",
                table: "Logs",
                columns: new[] { "Status", "StartedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleConfigurations_ActiveProviderConfigurationId",
                schema: "sms",
                table: "ModuleConfigurations",
                column: "ActiveProviderConfigurationId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleConfigurations_ModuleKey",
                schema: "sms",
                table: "ModuleConfigurations",
                column: "ModuleKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModuleConfigurations_PublicId",
                schema: "sms",
                table: "ModuleConfigurations",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderConfigurations_Name",
                schema: "sms",
                table: "ProviderConfigurations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderConfigurations_PublicId",
                schema: "sms",
                table: "ProviderConfigurations",
                column: "PublicId",
                unique: true);

            // Explicit data migration: keep legacy provider routing and the approved Kavenegar default.
            // Abort instead of silently assigning unsupported legacy provider names.
            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM [sms].[Messages] WHERE [Provider] NOT IN (N'SmsIr', N'Kavenegar'))
                    THROW 51000, 'Unknown legacy SMS provider; review routing before migrating.', 1;

                INSERT INTO [sms].[ProviderConfigurations]
                    ([Name], [Provider], [IsEnabled], [ApiKeyConfigurationKey], [TimeoutSeconds],
                     [TemplateMappingsJson], [PublicId], [IsDeleted], [CreatedAtUtc])
                VALUES
                    (N'SmsIr', N'SmsIr', 1, N'Sms:Providers:SmsIr:ApiKey', 30, N'{}', NEWID(), 0, SYSUTCDATETIME()),
                    (N'Kavenegar', N'Kavenegar', 1, N'Sms:Providers:Kavenegar:ApiKey', 30, N'{}', NEWID(), 0, SYSUTCDATETIME());

                UPDATE m SET [ProviderConfigurationId] = p.[Id]
                FROM [sms].[Messages] m
                INNER JOIN [sms].[ProviderConfigurations] p ON p.[Provider] = m.[Provider];

                INSERT INTO [sms].[ModuleConfigurations]
                    ([ModuleKey], [ActiveProviderConfigurationId], [IsEnabled], [BatchSize], [RetryEnabled],
                     [MaxAttempts], [InitialRetryDelaySeconds], [MaxRetryDelaySeconds], [PublicId], [IsDeleted], [CreatedAtUtc])
                SELECT N'Sms', [Id], 1, 100, 1, 3, 120, 3600, NEWID(), 0, SYSUTCDATETIME()
                FROM [sms].[ProviderConfigurations] WHERE [Name] = N'Kavenegar';
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ProviderConfigurations_ProviderConfigurationId",
                schema: "sms",
                table: "Messages",
                column: "ProviderConfigurationId",
                principalSchema: "sms",
                principalTable: "ProviderConfigurations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ProviderConfigurations_ProviderConfigurationId",
                schema: "sms",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "Logs",
                schema: "sms");

            migrationBuilder.DropTable(
                name: "ModuleConfigurations",
                schema: "sms");

            migrationBuilder.DropTable(
                name: "ProviderConfigurations",
                schema: "sms");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ProviderConfigurationId",
                schema: "sms",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ProviderConfigurationId",
                schema: "sms",
                table: "Messages");
        }
    }
}
