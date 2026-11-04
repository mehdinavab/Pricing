using Kahrad.Pricing.Guarantees.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Kahrad.Pricing.Api;

public sealed class GuaranteeExceptionHandler(
    ILogger<GuaranteeExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            GuaranteeOperationException => (StatusCodes.Status400BadRequest, "Guarantee operation failed"),
            _ => (0, string.Empty)
        };

        if (statusCode == 0)
            return false;

        logger.LogWarning(
            exception,
            "Guarantee request failed with status code {StatusCode}.",
            statusCode);

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = exception.Message
            },
            cancellationToken);

        return true;
    }
}
