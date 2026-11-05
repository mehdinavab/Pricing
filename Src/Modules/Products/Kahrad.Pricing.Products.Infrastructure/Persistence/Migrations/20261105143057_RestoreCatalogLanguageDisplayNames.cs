using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace Kahrad.Pricing.Products.Infrastructure.Persistence.Migrations;

public partial class RestoreCatalogLanguageDisplayNames : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Correct only known placeholder seed labels. Preserve manually supplied display names.
        // NCHAR makes these literals independent of shell and editor encodings.
        migrationBuilder.Sql("""
            UPDATE [products].[Languages]
            SET [Name] = NCHAR(0x0641) + NCHAR(0x0627) + NCHAR(0x0631) + NCHAR(0x0633) + NCHAR(0x06CC),
                [UpdatedAtUtc] = SYSUTCDATETIME()
            WHERE [Code] = N'fa' AND [Name] = N'?????';

            UPDATE [products].[Languages]
            SET [Name] = NCHAR(0x0627) + NCHAR(0x0644) + NCHAR(0x0639) + NCHAR(0x0631) + NCHAR(0x0628) + NCHAR(0x064A) + NCHAR(0x0629),
                [UpdatedAtUtc] = SYSUTCDATETIME()
            WHERE [Code] = N'ar' AND [Name] = N'???????';
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Retain corrected labels instead of deliberately restoring corrupted text.
    }
}
