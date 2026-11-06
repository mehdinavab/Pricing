namespace Kahrad.Pricing.Api;

/// <summary>
/// Applies consistent began, completed, and failed logging to every API operation.
/// </summary>
public sealed class ApiOperationLoggingMiddleware(
    RequestDelegate next,
    ILogger<ApiOperationLoggingMiddleware> logger)
{
    internal const string ExceptionLoggedKey = "Kahrad.Api.ExceptionLogged";

    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation(
            "API {Method} {Path} Began.",
            context.Request.Method,
            context.Request.Path);

        try
        {
            await next(context);

            logger.LogInformation(
                "API {Method} {Path} Done with status code {StatusCode}.",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode);
        }
        catch (Exception exception)
        {
            context.Items[ExceptionLoggedKey] = true;
            logger.LogError(
                exception,
                "API {Method} {Path} Failed.",
                context.Request.Method,
                context.Request.Path);
            throw;
        }
    }
}
