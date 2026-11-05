using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kahrad.Pricing.Representatives.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialRepresentatives : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "representatives");

            migrationBuilder.CreateTable(
                name: "Representatives",
                schema: "representatives",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<long>(type: "bigint", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_Representatives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountingMappings",
                schema: "representatives",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandId = table.Column<long>(type: "bigint", nullable: false),
                    RepresentativeId = table.Column<long>(type: "bigint", nullable: false),
                    AccountingRepresentativeId = table.Column<long>(type: "bigint", nullable: false),
                    MappedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_AccountingMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingMappings_Representatives_RepresentativeId",
                        column: x => x.RepresentativeId,
                        principalSchema: "representatives",
                        principalTable: "Representatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingMappings_BrandId_AccountingRepresentativeId",
                schema: "representatives",
                table: "AccountingMappings",
                columns: new[] { "BrandId", "AccountingRepresentativeId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingMappings_BrandId_RepresentativeId",
                schema: "representatives",
                table: "AccountingMappings",
                columns: new[] { "BrandId", "RepresentativeId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingMappings_PublicId",
                schema: "representatives",
                table: "AccountingMappings",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingMappings_RepresentativeId",
                schema: "representatives",
                table: "AccountingMappings",
                column: "RepresentativeId");

            migrationBuilder.CreateIndex(
                name: "IX_Representatives_BrandId_Code",
                schema: "representatives",
                table: "Representatives",
                columns: new[] { "BrandId", "Code" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Representatives_PublicId",
                schema: "representatives",
                table: "Representatives",
                column: "PublicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountingMappings",
                schema: "representatives");

            migrationBuilder.DropTable(
                name: "Representatives",
                schema: "representatives");
        }
    }
}
