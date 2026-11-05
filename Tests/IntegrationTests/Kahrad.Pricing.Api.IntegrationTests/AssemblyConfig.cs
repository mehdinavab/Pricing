using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests;
[CollectionDefinition("ApiTests", DisableParallelization = true)]
public sealed class ApiTestCollection : ICollectionFixture<KahradPricingApiWebAppFactory>;

[AttributeUsage(AttributeTargets.Method)]
public sealed class SqlServerFactAttribute : FactAttribute
{
    public SqlServerFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(KahradPricingApiWebAppFactory.ConnectionVariable)))
            Skip = "Set KAHRAD_TEST_SQL_SERVER to an authorized SQL Server, or run Tests/Run-ApiIntegrationTests.ps1.";
    }
}
[AttributeUsage(AttributeTargets.Method)]
public sealed class SqlServerTheoryAttribute : TheoryAttribute
{
    public SqlServerTheoryAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(KahradPricingApiWebAppFactory.ConnectionVariable)))
            Skip = "Set KAHRAD_TEST_SQL_SERVER to an authorized SQL Server, or run Tests/Run-ApiIntegrationTests.ps1.";
    }
}
