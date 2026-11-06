using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Kahrad.Pricing.Api.Localization;
using Kahrad.Pricing.Brands.Application.Exceptions;
using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;
using Kahrad.Pricing.Guarantees.Application.Exceptions;
using Kahrad.Pricing.Products.Application.Exceptions;
using Kahrad.Pricing.Sms.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Kahrad.Pricing.Api;

/// <summary>
/// Provides one error contract for every API module.
/// </summary>
public sealed class ApiExceptionHandler(
    ILogger<ApiExceptionHandler> logger,
    IStringLocalizer<ApiMessages> messages) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var failure = Classify(exception);

        if (!context.Items.ContainsKey(ApiOperationLoggingMiddleware.ExceptionLoggedKey))
        {
            logger.LogError(
                exception,
                "API {Method} {Path} Failed with error code {ErrorCode}.",
                context.Request.Method,
                context.Request.Path,
                (int)failure.ErrorCode);
        }

        if (failure.IsSystemError)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "text/plain; charset=utf-8";
            await context.Response.WriteAsync(
                messages["The server encountered an error."],
                cancellationToken);
            return true;
        }

        var title = ApiText.Translate(messages, GetTitle(exception, failure.ErrorCode), "The request is invalid.");
        var detail = ApiText.Translate(messages, exception.Message, GetTitle(exception, failure.ErrorCode));
        var statusCode = GetHttpStatusCode(failure.ErrorCode);
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };
        problem.Extensions["errorCode"] = (int)failure.ErrorCode;

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private static FailureClassification Classify(Exception exception) => exception switch
    {
        KahradPricingException known => new(known.IsSystemError, known.ErrorCode),
        KeyNotFoundException => new(false, ErrorCodes.NotFound),
        UnauthorizedAccessException => new(false, ErrorCodes.Unauthorized),
        DbUpdateConcurrencyException => new(false, ErrorCodes.Conflict),
        _ => new(true, ErrorCodes.SystemError)
    };

    private static int GetHttpStatusCode(ErrorCodes errorCode) => errorCode switch
    {
        ErrorCodes.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorCodes.Forbidden => StatusCodes.Status403Forbidden,
        ErrorCodes.NotFound => StatusCodes.Status404NotFound,
        ErrorCodes.Conflict => StatusCodes.Status409Conflict,
        ErrorCodes.ServiceUnavailable => StatusCodes.Status503ServiceUnavailable,
        _ => StatusCodes.Status400BadRequest
    };

    private static string GetTitle(Exception exception, ErrorCodes errorCode) => exception switch
    {
        BrandNotFoundException => "Brand not found",
        StoreNotFoundException => "Store not found",
        VendorNotFoundException => "Vendor not found",
        GuaranteeNotFoundException => "Guarantee not found",
        CertificateNotFoundException => "Guarantee certificate not found",
        ProductNotFoundException => "Product not found",
        ProductGroupNotFoundException => "Product group not found",
        ProductFeatureNotFoundException => "Product feature not found",
        ProductImageNotFoundException => "Product image not found",
        SmsNotFoundException => "SMS message not found",
        AccountingProviderNotConfiguredException => "Accounting provider is not configured.",
        _ => errorCode switch
        {
            ErrorCodes.Validation => "The request is invalid.",
            ErrorCodes.Unauthorized => "Unauthorized",
            ErrorCodes.Forbidden => "Forbidden",
            ErrorCodes.NotFound => "Resource not found",
            ErrorCodes.Conflict => "The resource was changed by another request",
            ErrorCodes.ServiceUnavailable => "The service is unavailable.",
            _ => "The request is invalid."
        }
    };

    private readonly record struct FailureClassification(bool IsSystemError, ErrorCodes ErrorCode);
}
