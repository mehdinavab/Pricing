using Kahrad.Pricing.Identity.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Api;

public sealed class IdentityExceptionHandler(
    ILogger<IdentityExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
            IdentityOperationException => (StatusCodes.Status400BadRequest, "Identity operation failed"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "The resource was changed by another request"),
            _ => (0, string.Empty)
        };

        if (statusCode == 0)
            return false;

        logger.LogWarning(
            exception,
            "Identity request failed with status code {StatusCode}.",
            statusCode);

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = statusCode == StatusCodes.Status401Unauthorized
                    ? null
                    : exception.Message
            },
            cancellationToken);

        return true;
    }
}
