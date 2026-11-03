using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kahrad.Pricing.Api;

public sealed class FluentValidationActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (context.HttpContext.RequestServices.GetService(validatorType)
                is not IValidator validator)
            {
                continue;
            }

            var validationResult = await validator.ValidateAsync(
                new ValidationContext<object>(argument),
                context.HttpContext.RequestAborted);

            if (validationResult.IsValid)
                continue;

            var errors = validationResult.Errors
                .GroupBy(x => string.IsNullOrWhiteSpace(x.PropertyName)
                    ? "Request"
                    : x.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(x => x.ErrorMessage).ToArray());

            context.Result = new BadRequestObjectResult(
                new ValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest
                });
            return;
        }

        await next();
    }
}
