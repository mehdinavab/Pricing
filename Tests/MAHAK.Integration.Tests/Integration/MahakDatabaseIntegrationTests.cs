using Dapper;
using Microsoft.Data.SqlClient;
using Xunit;

namespace MAHAK.Integration.Tests.Integration;

public sealed class MahakDatabaseIntegrationTests
{
    [SqlServerFact]
    public async Task Explicit_test_database_is_reachable_and_contains_required_tables()
    {
        var connectionString = Environment.GetEnvironmentVariable("MAHAK_TEST_SQL_SERVER")!;
        await using var connection = new SqlConnection(connectionString);
        const string sql = """
            SELECT COUNT_BIG(1)
            FROM sys.tables
            WHERE name IN (
                'GoodInf', 'PerInf', 'Fact_Fo', 'Fact_Fo_Detail',
                'BackFact_Detail', 'GoodCostDiscount'
            );
            """;

        var count = await connection.ExecuteScalarAsync<long>(sql);

        Assert.Equal(6, count);
    }
}
