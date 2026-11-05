using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kahrad.Pricing.AccountingIntegration.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRunningSyncUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SyncRuns_BrandId_Type",
                schema: "accounting",
                table: "SyncRuns",
                columns: new[] { "BrandId", "Type" },
                unique: true,
                filter: "[Status] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SyncRuns_BrandId_Type",
                schema: "accounting",
                table: "SyncRuns");
        }
    }
}
