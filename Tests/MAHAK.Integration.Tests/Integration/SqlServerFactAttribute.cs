using Xunit;

namespace MAHAK.Integration.Tests.Integration;

public sealed class SqlServerFactAttribute : FactAttribute
{
    public SqlServerFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("MAHAK_TEST_SQL_SERVER")))
        {
            Skip = "Set MAHAK_TEST_SQL_SERVER to a dedicated, schema-compatible MAHAK test database. Never use production.";
        }
    }
}
