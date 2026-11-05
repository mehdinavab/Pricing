using Kahrad.Pricing.AccountingIntegration.Application;
using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Kahrad.Pricing.Api.Localization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Kahrad.Pricing.Api;

public sealed class AccountingExceptionHandler(IStringLocalizer<ApiMessages> messages,
    ILogger<AccountingExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            AccountingProviderNotConfiguredException => (StatusCodes.Status503ServiceUnavailable, "Accounting provider is not configured."),
            AccountingOperationException => (StatusCodes.Status400BadRequest, "Accounting operation failed"),
            _ => (0, string.Empty)
        };
        if (status == 0) return false;
        logger.LogWarning("Accounting request failed with status code {StatusCode}.", status);
        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = ApiText.Translate(messages, title, "Accounting operation failed"),
            Detail = exception is AccountingProviderNotConfiguredException ? null :
                ApiText.Translate(messages, exception.Message, title)
        }, ct);
        return true;
    }
}
