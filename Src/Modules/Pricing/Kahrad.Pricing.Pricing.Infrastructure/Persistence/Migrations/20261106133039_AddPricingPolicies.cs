using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kahrad.Pricing.Pricing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingPolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrandPricingPolicies",
                schema: "pricing",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<long>(type: "bigint", nullable: false),
                    BeautyPercentage = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    FactorySalesPercentage = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    CommercePercentage = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    RepresentativeFactor = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    FactoryRoundingStep = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
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
                    table.PrimaryKey("PK_BrandPricingPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepresentativePricingPolicies",
                schema: "pricing",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<long>(type: "bigint", nullable: false),
                    RepresentativeId = table.Column<long>(type: "bigint", nullable: false),
                    SalesPercentage = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    GeneralDiscountPercentage = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    CustomerRoundingStep = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
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
                    table.PrimaryKey("PK_RepresentativePricingPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepresentativeProductDiscounts",
                schema: "pricing",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<long>(type: "bigint", nullable: false),
                    RepresentativeId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
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
                    table.PrimaryKey("PK_RepresentativeProductDiscounts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandPricingPolicies_BrandId",
                schema: "pricing",
                table: "BrandPricingPolicies",
                column: "BrandId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BrandPricingPolicies_PublicId",
                schema: "pricing",
                table: "BrandPricingPolicies",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RepresentativePricingPolicies_BrandId_RepresentativeId",
                schema: "pricing",
                table: "RepresentativePricingPolicies",
                columns: new[] { "BrandId", "RepresentativeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RepresentativePricingPolicies_PublicId",
                schema: "pricing",
                table: "RepresentativePricingPolicies",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RepresentativeProductDiscounts_BrandId_ProductId",
                schema: "pricing",
                table: "RepresentativeProductDiscounts",
                columns: new[] { "BrandId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_RepresentativeProductDiscounts_BrandId_RepresentativeId_ProductId",
                schema: "pricing",
                table: "RepresentativeProductDiscounts",
                columns: new[] { "BrandId", "RepresentativeId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RepresentativeProductDiscounts_PublicId",
                schema: "pricing",
                table: "RepresentativeProductDiscounts",
                column: "PublicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandPricingPolicies",
                schema: "pricing");

            migrationBuilder.DropTable(
                name: "RepresentativePricingPolicies",
                schema: "pricing");

            migrationBuilder.DropTable(
                name: "RepresentativeProductDiscounts",
                schema: "pricing");
        }
    }
}
