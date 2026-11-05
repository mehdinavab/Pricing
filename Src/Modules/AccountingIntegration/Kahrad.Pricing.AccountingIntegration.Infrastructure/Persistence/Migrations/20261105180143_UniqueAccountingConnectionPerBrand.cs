using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kahrad.Pricing.AccountingIntegration.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UniqueAccountingConnectionPerBrand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BrandConnections_BrandId_ProviderCode",
                schema: "accounting",
                table: "BrandConnections");

            migrationBuilder.CreateIndex(
                name: "IX_BrandConnections_BrandId",
                schema: "accounting",
                table: "BrandConnections",
                column: "BrandId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BrandConnections_BrandId",
                schema: "accounting",
                table: "BrandConnections");

            migrationBuilder.CreateIndex(
                name: "IX_BrandConnections_BrandId_ProviderCode",
                schema: "accounting",
                table: "BrandConnections",
                columns: new[] { "BrandId", "ProviderCode" },
                unique: true);
        }
    }
}
