using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kahrad.Pricing.Pricing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialPriceOverrides : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "pricing");

            migrationBuilder.CreateTable(
                name: "ProductFactoryPriceOverrides",
                schema: "pricing",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Mode = table.Column<int>(type: "int", nullable: false),
                    FixedFactorySellingPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ManualFactoryPriceAdjustment = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
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
                    table.PrimaryKey("PK_ProductFactoryPriceOverrides", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductFactoryPriceOverrides_BrandId_ProductId",
                schema: "pricing",
                table: "ProductFactoryPriceOverrides",
                columns: new[] { "BrandId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductFactoryPriceOverrides_PublicId",
                schema: "pricing",
                table: "ProductFactoryPriceOverrides",
                column: "PublicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductFactoryPriceOverrides",
                schema: "pricing");
        }
    }
}
