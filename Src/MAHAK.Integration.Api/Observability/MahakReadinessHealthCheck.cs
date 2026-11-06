using Dapper;
using MAHAK.Integration.Api.Data;
using MAHAK.Integration.Api.Security;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MAHAK.Integration.Api.Observability;

public sealed class MahakReadinessHealthCheck(
    IBrandCatalog brands,
    ISqlConnectionFactory connections) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (brands.EnabledBrands.Count == 0)
            return HealthCheckResult.Unhealthy("No enabled MAHAK brand is configured.");

        try
        {
            foreach (var brand in brands.EnabledBrands)
            {
                await using var connection = connections.Create(brand);
                var command = new CommandDefinition(
                    "SELECT 1;",
                    commandTimeout: Math.Min(brand.CommandTimeoutSeconds, 5),
                    cancellationToken: cancellationToken);
                await connection.ExecuteScalarAsync<int>(command);
            }
            return HealthCheckResult.Healthy("All configured MAHAK databases are reachable.");
        }
        catch
        {
            return HealthCheckResult.Unhealthy(
                "One or more configured MAHAK databases are unavailable.");
        }
    }
}
