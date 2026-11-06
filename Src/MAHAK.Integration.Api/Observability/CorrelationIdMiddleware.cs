using System.Diagnostics;

namespace MAHAK.Integration.Api.Observability;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context, ILogger<CorrelationIdMiddleware> logger)
    {
        var requested = context.Request.Headers[HeaderName].ToString();
        var correlationId = IsSafe(requested)
            ? requested
            : Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        context.TraceIdentifier = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        var started = Stopwatch.GetTimestamp();
        try
        {
            await next(context);
        }
        finally
        {
            var brandCode = context.User.FindFirst("mahak_brand")?.Value ?? "anonymous";
            logger.LogInformation(
                "HTTP {Method} {Path} completed with {StatusCode} for brand {BrandCode} in {ElapsedMilliseconds:F1} ms; correlation {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                brandCode,
                Stopwatch.GetElapsedTime(started).TotalMilliseconds,
                correlationId);
        }
    }

    private static bool IsSafe(string value) =>
        value.Length is > 0 and <= 128 && value.All(character => !char.IsControl(character));
}
