using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace MAHAK.Integration.Api.Errors;

public sealed class ApiExceptionHandler(
    IProblemDetailsService problemDetails,
    ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title, detail) = exception switch
        {
            RequestValidationException => (400, "The request is invalid.", exception.Message),
            BadHttpRequestException => (400, "The request is invalid.", "The request body could not be accepted."),
            JsonException => (400, "The request is invalid.", "The request body could not be accepted."),
            AccountingRecordNotFoundException => (404, "The accounting record was not found.", exception.Message),
            SchemaConfirmationRequiredException => (409, "MAHAK schema confirmation is required.", exception.Message),
            AccountingDataIntegrityException => (409, "MAHAK data integrity prevented the operation.", exception.Message),
            SqlException => (503, "The MAHAK database is temporarily unavailable.", null),
            TimeoutException => (503, "The MAHAK database operation timed out.", null),
            _ => (500, "An unexpected server error occurred.", null)
        };

        if (status == StatusCodes.Status503ServiceUnavailable)
        {
            logger.LogWarning(
                "MAHAK dependency failure with category {ErrorCategory} and correlation {CorrelationId}",
                exception.GetType().Name,
                httpContext.TraceIdentifier);
        }
        else if (status >= 500)
        {
            logger.LogError(
                exception,
                "MAHAK request failed with category {ErrorCategory} and correlation {CorrelationId}",
                exception.GetType().Name,
                httpContext.TraceIdentifier);
        }

        httpContext.Response.StatusCode = status;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Extensions = { ["correlationId"] = httpContext.TraceIdentifier }
            },
            Exception = exception
        });
    }
}
