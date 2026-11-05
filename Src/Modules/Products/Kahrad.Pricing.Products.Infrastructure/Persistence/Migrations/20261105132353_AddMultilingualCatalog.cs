using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kahrad.Pricing.Products.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMultilingualCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FeatureId",
                schema: "products",
                table: "ProductFeatures",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Features",
                schema: "products",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
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
                    table.PrimaryKey("PK_Features", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                schema: "products",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false, collation: "Latin1_General_100_CI_AS"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_Languages", x => x.Id);
                    table.CheckConstraint("CK_Language_DefaultActive", "[IsDefault] = 0 OR ([IsActive] = 1 AND [IsDeleted] = 0)");
                });

            migrationBuilder.CreateTable(
                name: "Specifications",
                schema: "products",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ValueType = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_Specifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeatureTranslations",
                schema: "products",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeatureId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
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
                    table.PrimaryKey("PK_FeatureTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeatureTranslations_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalSchema: "products",
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeatureTranslations_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "products",
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductTranslations",
                schema: "products",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LocalizedDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_ProductTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductTranslations_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "products",
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductTranslations_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "products",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductSpecifications",
                schema: "products",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    SpecificationId = table.Column<long>(type: "bigint", nullable: false),
                    TextValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NumberValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    BooleanValue = table.Column<bool>(type: "bit", nullable: true),
                    DateValue = table.Column<DateTime>(type: "date", nullable: true),
                    CodeValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_ProductSpecifications", x => x.Id);
                    table.CheckConstraint("CK_ProductSpecification_OneValue", "(CASE WHEN [TextValue] IS NULL THEN 0 ELSE 1 END + CASE WHEN [NumberValue] IS NULL THEN 0 ELSE 1 END + CASE WHEN [BooleanValue] IS NULL THEN 0 ELSE 1 END + CASE WHEN [DateValue] IS NULL THEN 0 ELSE 1 END + CASE WHEN [CodeValue] IS NULL THEN 0 ELSE 1 END) = 1");
                    table.ForeignKey(
                        name: "FK_ProductSpecifications_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "products",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSpecifications_Specifications_SpecificationId",
                        column: x => x.SpecificationId,
                        principalSchema: "products",
                        principalTable: "Specifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SpecificationTranslations",
                schema: "products",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecificationId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
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
                    table.PrimaryKey("PK_SpecificationTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecificationTranslations_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "products",
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SpecificationTranslations_Specifications_SpecificationId",
                        column: x => x.SpecificationId,
                        principalSchema: "products",
                        principalTable: "Specifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductSpecificationTranslations",
                schema: "products",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductSpecificationId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
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
                    table.PrimaryKey("PK_ProductSpecificationTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSpecificationTranslations_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "products",
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductSpecificationTranslations_ProductSpecifications_ProductSpecificationId",
                        column: x => x.ProductSpecificationId,
                        principalSchema: "products",
                        principalTable: "ProductSpecifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeatures_FeatureId",
                schema: "products",
                table: "ProductFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Features_Code",
                schema: "products",
                table: "Features",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Features_PublicId",
                schema: "products",
                table: "Features",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeatureTranslations_FeatureId_LanguageId",
                schema: "products",
                table: "FeatureTranslations",
                columns: new[] { "FeatureId", "LanguageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeatureTranslations_LanguageId",
                schema: "products",
                table: "FeatureTranslations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureTranslations_PublicId",
                schema: "products",
                table: "FeatureTranslations",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Code",
                schema: "products",
                table: "Languages",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_IsDefault",
                schema: "products",
                table: "Languages",
                column: "IsDefault",
                unique: true,
                filter: "[IsDefault] = 1 AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_PublicId",
                schema: "products",
                table: "Languages",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_ProductId_SpecificationId",
                schema: "products",
                table: "ProductSpecifications",
                columns: new[] { "ProductId", "SpecificationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_PublicId",
                schema: "products",
                table: "ProductSpecifications",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecifications_SpecificationId",
                schema: "products",
                table: "ProductSpecifications",
                column: "SpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecificationTranslations_LanguageId",
                schema: "products",
                table: "ProductSpecificationTranslations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecificationTranslations_ProductSpecificationId_LanguageId",
                schema: "products",
                table: "ProductSpecificationTranslations",
                columns: new[] { "ProductSpecificationId", "LanguageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSpecificationTranslations_PublicId",
                schema: "products",
                table: "ProductSpecificationTranslations",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTranslations_LanguageId",
                schema: "products",
                table: "ProductTranslations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTranslations_ProductId_LanguageId",
                schema: "products",
                table: "ProductTranslations",
                columns: new[] { "ProductId", "LanguageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductTranslations_PublicId",
                schema: "products",
                table: "ProductTranslations",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specifications_Code",
                schema: "products",
                table: "Specifications",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specifications_PublicId",
                schema: "products",
                table: "Specifications",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationTranslations_LanguageId",
                schema: "products",
                table: "SpecificationTranslations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationTranslations_PublicId",
                schema: "products",
                table: "SpecificationTranslations",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationTranslations_SpecificationId_LanguageId",
                schema: "products",
                table: "SpecificationTranslations",
                columns: new[] { "SpecificationId", "LanguageId" },
                unique: true);

            // Historical product and feature labels were explicitly confirmed as Persian by the owner.
            // Keep all legacy content, IDs, assignment rows, values, ordering and price effects intact.
            migrationBuilder.Sql("""
                INSERT INTO [products].[Languages] ([Code], [Name], [IsActive], [IsDefault], [PublicId], [IsDeleted], [CreatedAtUtc])
                SELECT v.[Code], v.[Name], 1, v.[IsDefault], NEWID(), 0, SYSUTCDATETIME()
                FROM (VALUES (N'fa', N'?????', CAST(1 AS bit)), (N'ar', N'???????', CAST(0 AS bit)), (N'en', N'English', CAST(0 AS bit))) v([Code], [Name], [IsDefault])
                WHERE NOT EXISTS (SELECT 1 FROM [products].[Languages] l WHERE l.[Code] = v.[Code]);

                DECLARE @fa bigint = (SELECT [Id] FROM [products].[Languages] WHERE [Code] = N'fa');
                INSERT INTO [products].[ProductTranslations]
                    ([ProductId], [LanguageId], [Name], [LocalizedDescription], [PublicId], [IsDeleted], [CreatedAtUtc], [CreatedById])
                SELECT p.[Id], @fa, p.[Name], p.[Description], NEWID(), 0, p.[CreatedAtUtc], p.[CreatedById]
                FROM [products].[Products] p
                WHERE NULLIF(LTRIM(RTRIM(p.[Name])), N'') IS NOT NULL
                  AND NOT EXISTS (SELECT 1 FROM [products].[ProductTranslations] t WHERE t.[ProductId] = p.[Id] AND t.[LanguageId] = @fa);

                -- Matching names alone do not establish semantic identity: retain one definition per legacy assignment.
                -- Future assignments can explicitly reuse any definition; translations are shared by definition ID.
                INSERT INTO [products].[Features] ([Code], [Name], [PublicId], [IsDeleted], [CreatedAtUtc], [CreatedById])
                SELECT N'LEGACY-' + CONVERT(nvarchar(20), f.[Id]), f.[Name], NEWID(), f.[IsDeleted], f.[CreatedAtUtc], f.[CreatedById]
                FROM [products].[ProductFeatures] f
                WHERE NULLIF(LTRIM(RTRIM(f.[Name])), N'') IS NOT NULL
                  AND NOT EXISTS (SELECT 1 FROM [products].[Features] d WHERE d.[Code] = N'LEGACY-' + CONVERT(nvarchar(20), f.[Id]));
                UPDATE f SET [FeatureId] = d.[Id]
                FROM [products].[ProductFeatures] f JOIN [products].[Features] d
                  ON d.[Code] = N'LEGACY-' + CONVERT(nvarchar(20), f.[Id])
                WHERE f.[FeatureId] IS NULL;
                INSERT INTO [products].[FeatureTranslations] ([FeatureId], [LanguageId], [Name], [PublicId], [IsDeleted], [CreatedAtUtc], [CreatedById])
                SELECT f.[Id], @fa, f.[Name], NEWID(), 0, f.[CreatedAtUtc], f.[CreatedById]
                FROM [products].[Features] f
                WHERE NOT EXISTS (SELECT 1 FROM [products].[FeatureTranslations] t WHERE t.[FeatureId] = f.[Id] AND t.[LanguageId] = @fa);
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFeatures_Features_FeatureId",
                schema: "products",
                table: "ProductFeatures",
                column: "FeatureId",
                principalSchema: "products",
                principalTable: "Features",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductFeatures_Features_FeatureId",
                schema: "products",
                table: "ProductFeatures");

            migrationBuilder.DropTable(
                name: "FeatureTranslations",
                schema: "products");

            migrationBuilder.DropTable(
                name: "ProductSpecificationTranslations",
                schema: "products");

            migrationBuilder.DropTable(
                name: "ProductTranslations",
                schema: "products");

            migrationBuilder.DropTable(
                name: "SpecificationTranslations",
                schema: "products");

            migrationBuilder.DropTable(
                name: "Features",
                schema: "products");

            migrationBuilder.DropTable(
                name: "ProductSpecifications",
                schema: "products");

            migrationBuilder.DropTable(
                name: "Languages",
                schema: "products");

            migrationBuilder.DropTable(
                name: "Specifications",
                schema: "products");

            migrationBuilder.DropIndex(
                name: "IX_ProductFeatures_FeatureId",
                schema: "products",
                table: "ProductFeatures");

            migrationBuilder.DropColumn(
                name: "FeatureId",
                schema: "products",
                table: "ProductFeatures");
        }
    }
}
