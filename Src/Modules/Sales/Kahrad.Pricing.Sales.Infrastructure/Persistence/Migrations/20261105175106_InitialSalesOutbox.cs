using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kahrad.Pricing.Sales.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSalesOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sales");

            migrationBuilder.CreateTable(
                name: "SoldProductUnits",
                schema: "sales",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    OperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    RepresentativeId = table.Column<long>(type: "bigint", nullable: false),
                    ProductionCostSnapshot = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    RepresentativePriceSnapshot = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CustomerSellingPriceSnapshot = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    PublishedPriceVersion = table.Column<long>(type: "bigint", nullable: false),
                    FinalizedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoldProductUnits", x => x.Id);
                    table.UniqueConstraint("AK_SoldProductUnits_PublicId", x => x.PublicId);
                });

            migrationBuilder.CreateTable(
                name: "AccountingSalePostOutbox",
                schema: "sales",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoldProductUnitPublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandId = table.Column<long>(type: "bigint", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    NextAttemptAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastAttemptAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExternalReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ErrorCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_AccountingSalePostOutbox", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingSalePostOutbox_SoldProductUnits_SoldProductUnitPublicId",
                        column: x => x.SoldProductUnitPublicId,
                        principalSchema: "sales",
                        principalTable: "SoldProductUnits",
                        principalColumn: "PublicId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSalePostOutbox_IdempotencyKey",
                schema: "sales",
                table: "AccountingSalePostOutbox",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSalePostOutbox_PublicId",
                schema: "sales",
                table: "AccountingSalePostOutbox",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSalePostOutbox_SoldProductUnitPublicId",
                schema: "sales",
                table: "AccountingSalePostOutbox",
                column: "SoldProductUnitPublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingSalePostOutbox_Status_NextAttemptAtUtc",
                schema: "sales",
                table: "AccountingSalePostOutbox",
                columns: new[] { "Status", "NextAttemptAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SoldProductUnits_BrandId_ProductId_FinalizedAtUtc",
                schema: "sales",
                table: "SoldProductUnits",
                columns: new[] { "BrandId", "ProductId", "FinalizedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SoldProductUnits_OperationId",
                schema: "sales",
                table: "SoldProductUnits",
                column: "OperationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SoldProductUnits_PublicId",
                schema: "sales",
                table: "SoldProductUnits",
                column: "PublicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountingSalePostOutbox",
                schema: "sales");

            migrationBuilder.DropTable(
                name: "SoldProductUnits",
                schema: "sales");
        }
    }
}
