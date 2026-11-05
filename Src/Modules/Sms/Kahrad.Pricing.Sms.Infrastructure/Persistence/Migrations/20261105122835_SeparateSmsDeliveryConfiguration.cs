using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kahrad.Pricing.Sms.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeparateSmsDeliveryConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxRetries",
                schema: "sms",
                table: "Messages");

            migrationBuilder.AddColumn<DateTime>(
                name: "NextAttemptAtUtc",
                schema: "sms",
                table: "Messages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                schema: "sms",
                table: "Messages",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "SmsIr");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Status_NextAttemptAtUtc_SendDateUtc",
                schema: "sms",
                table: "Messages",
                columns: new[] { "Status", "NextAttemptAtUtc", "SendDateUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_Status_NextAttemptAtUtc_SendDateUtc",
                schema: "sms",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "NextAttemptAtUtc",
                schema: "sms",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Provider",
                schema: "sms",
                table: "Messages");

            migrationBuilder.AddColumn<int>(
                name: "MaxRetries",
                schema: "sms",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 3);
        }
    }
}
