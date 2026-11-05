using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Formatting.Json;
namespace Kahrad.Pricing.BuildingBlocks.Infrastructure.Hosting;

public static class HostOperations
{
    public static IServiceCollection AddHostLogging(this IServiceCollection services, IConfiguration configuration, string application)
    {
        services.AddSerilog((_, logger) => logger
            .MinimumLevel.Information().MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .ReadFrom.Configuration(configuration).Enrich.FromLogContext().Enrich.WithProperty("Application", application)
            .WriteTo.Console(new JsonFormatter(renderMessage: true)));
        return services;
    }

    public static IServiceCollection AddApiHealth(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }

    public static IServiceCollection AddRequiredSqlReadiness(this IServiceCollection services, string name, string? connectionString)
    {
        services.AddHealthChecks().AddCheck(name, new RequiredSqlHealthCheck(connectionString), tags: ["ready"]);
        return services;
    }

    public static WebApplication UseHostRequestLogging(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            var requested = context.Request.Headers["X-Correlation-ID"].ToString();
            var correlation = requested.Length is > 0 and <= 64 && Regex.IsMatch(requested, "^[A-Za-z0-9_-]+$")
                ? requested : context.TraceIdentifier;
            context.TraceIdentifier = correlation;
            context.Response.Headers["X-Correlation-ID"] = correlation;
            using (LogContext.PushProperty("CorrelationId", correlation)) await next(context);
        });
        app.UseSerilogRequestLogging(options =>
            options.GetLevel = (context, elapsed, exception) => exception is not null || context.Response.StatusCode >= 500
                ? LogEventLevel.Error : context.Request.Path.StartsWithSegments("/health") ? LogEventLevel.Debug : LogEventLevel.Information);
        return app;
    }

    public static WebApplication MapHostHealth(this WebApplication app)
    {
        static Task WriteSafeHealth(HttpContext context, HealthReport report) =>
            context.Response.WriteAsJsonAsync(new { status = report.Status.ToString() }, context.RequestAborted);
        app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false, ResponseWriter = WriteSafeHealth }).AllowAnonymous();
        app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = x => x.Tags.Contains("ready"), ResponseWriter = WriteSafeHealth }).AllowAnonymous();
        app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = x => x.Tags.Contains("ready"), ResponseWriter = WriteSafeHealth }).AllowAnonymous();
        return app;
    }
}

public sealed class RequiredSqlHealthCheck(string? connectionString) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) return HealthCheckResult.Unhealthy("A required data store is not configured.");
        try
        {
            var settings = new SqlConnectionStringBuilder(connectionString) { ConnectTimeout = 3 };
            await using var connection = new SqlConnection(settings.ConnectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1"; command.CommandTimeout = 3;
            await command.ExecuteScalarAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex) when (ex is SqlException or InvalidOperationException or ArgumentException)
        {
            return HealthCheckResult.Unhealthy("A required data store is unavailable.");
        }
    }
}
