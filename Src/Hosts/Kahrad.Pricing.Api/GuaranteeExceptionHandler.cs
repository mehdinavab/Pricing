using Kahrad.Pricing.Guarantees.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Kahrad.Pricing.Api.Localization;
using Microsoft.Extensions.Localization;

namespace Kahrad.Pricing.Api;

public sealed class GuaranteeExceptionHandler(
    ILogger<GuaranteeExceptionHandler> logger, IStringLocalizer<ApiMessages> messages) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            VendorNotFoundException => (StatusCodes.Status404NotFound, "Vendor not found"),
            GuaranteeNotFoundException => (StatusCodes.Status404NotFound, "Guarantee not found"),
            CertificateNotFoundException => (StatusCodes.Status404NotFound, "Guarantee certificate not found"),
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
                Title = ApiText.Translate(messages, title, "Guarantee operation failed"),
                Detail = ApiText.Translate(messages, exception.Message, title)
            },
            cancellationToken);

        return true;
    }
}
