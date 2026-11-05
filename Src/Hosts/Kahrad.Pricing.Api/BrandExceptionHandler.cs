using Kahrad.Pricing.Brands.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Kahrad.Pricing.Api.Localization;
using Microsoft.Extensions.Localization;

namespace Kahrad.Pricing.Api;

public sealed class BrandExceptionHandler(
    ILogger<BrandExceptionHandler> logger, IStringLocalizer<ApiMessages> messages) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            BrandNotFoundException => (StatusCodes.Status404NotFound, "Brand not found"),
            StoreNotFoundException => (StatusCodes.Status404NotFound, "Store not found"),
            BrandOperationException => (StatusCodes.Status400BadRequest, "Brand operation failed"),
            _ => (0, string.Empty)
        };

        if (statusCode == 0)
            return false;

        logger.LogWarning(
            exception,
            "Brand request failed with status code {StatusCode}.",
            statusCode);

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = statusCode,
                Title = ApiText.Translate(messages, title, "Brand operation failed"),
                Detail = ApiText.Translate(messages, exception.Message, title)
            },
            cancellationToken);

        return true;
    }
}
