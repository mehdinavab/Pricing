using Kahrad.Pricing.Products.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Kahrad.Pricing.Api.Localization;
using Microsoft.Extensions.Localization;

namespace Kahrad.Pricing.Api;

public sealed class ProductExceptionHandler(
    ILogger<ProductExceptionHandler> logger, IStringLocalizer<ApiMessages> messages) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            Kahrad.Pricing.Products.Domain.Exceptions.CatalogRuleException => (StatusCodes.Status400BadRequest, "Catalog validation failed"),
            CatalogConflictException => (StatusCodes.Status409Conflict, "Catalog update conflict"),
            ProductOperationException => (StatusCodes.Status400BadRequest, "Product operation failed"),
            ProductNotFoundException => (StatusCodes.Status404NotFound, "Product not found"),
            ProductGroupNotFoundException => (StatusCodes.Status404NotFound, "Product group not found"),
            ProductFeatureNotFoundException => (StatusCodes.Status404NotFound, "Product feature not found"),
            ProductImageNotFoundException => (StatusCodes.Status404NotFound, "Product image not found"),
            _ => (0, string.Empty)
        };

        if (statusCode == 0)
            return false;

        logger.LogWarning(
            exception,
            "Product request failed with status code {StatusCode}.",
            statusCode);

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = statusCode,
                Title = ApiText.Translate(messages, title, "Product operation failed"),
                Detail = ApiText.Translate(messages, exception.Message, title)
            },
            cancellationToken);

        return true;
    }
}
