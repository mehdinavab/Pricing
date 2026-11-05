using Kahrad.Pricing.Api.Localization;
using Kahrad.Pricing.Pricing.Application;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Kahrad.Pricing.Api;

public sealed class PricingExceptionHandler(IStringLocalizer<ApiMessages> messages) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not PricingOperationException pricing) return false;
        var status = pricing.IsConflict ? StatusCodes.Status409Conflict : StatusCodes.Status400BadRequest;
        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = messages[pricing.IsConflict ? "Pricing update conflict" : "Pricing operation failed"],
            Detail = ApiText.Translate(messages, pricing.Message, "Pricing operation failed")
        }, ct);
        return true;
    }
}
