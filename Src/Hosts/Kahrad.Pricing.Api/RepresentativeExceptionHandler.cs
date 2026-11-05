using Kahrad.Pricing.Api.Localization;
using Kahrad.Pricing.Representatives.Application;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Kahrad.Pricing.Api;

public sealed class RepresentativeExceptionHandler(IStringLocalizer<ApiMessages> messages) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not RepresentativeOperationException) return false;
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = messages["Representative operation failed"],
            Detail = ApiText.Translate(messages, exception.Message, "Representative operation failed")
        }, ct);
        return true;
    }
}
