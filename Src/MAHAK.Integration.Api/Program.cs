using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using MAHAK.Integration.Api.Configuration;
using MAHAK.Integration.Api.Data;
using MAHAK.Integration.Api.Data.Repositories;
using MAHAK.Integration.Api.Endpoints;
using MAHAK.Integration.Api.Errors;
using MAHAK.Integration.Api.Observability;
using MAHAK.Integration.Api.Security;
using MAHAK.Integration.Api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 64 * 1024);
builder.Services.Configure<IISServerOptions>(options => options.MaxRequestBodySize = 64 * 1024);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services
    .AddOptions<MahakOptions>()
    .BindConfiguration(MahakOptions.SectionName)
    .ValidateOnStart();
builder.Services.AddSingleton<IValidateOptions<MahakOptions>, MahakOptionsValidator>();

builder.Services.AddSingleton<ApiKeyBrandResolver>();
builder.Services.AddSingleton<IApiKeyBrandResolver>(services =>
    services.GetRequiredService<ApiKeyBrandResolver>());
builder.Services.AddSingleton<IBrandCatalog>(services =>
    services.GetRequiredService<ApiKeyBrandResolver>());
builder.Services.AddSingleton<IBrandContextAccessor, BrandContextAccessor>();
builder.Services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IRepresentativeRepository, RepresentativeRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<ISalePriceRepository, SalePriceRepository>();
builder.Services.AddScoped<BoundedSyncPolicy>();
builder.Services.AddScoped<ItemService>();
builder.Services.AddScoped<RepresentativeService>();
builder.Services.AddScoped<InvoiceService>();
builder.Services.AddScoped<SalePriceService>();

builder.Services.AddAuthentication(ApiKeyAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
        ApiKeyAuthenticationHandler.SchemeName,
        _ => { });
builder.Services.AddAuthorization();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddOpenApi(options =>
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        MahakOpenApi.AddApiKeySecurity(document);
        return Task.CompletedTask;
    }));
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/problem+json";
        await context.HttpContext.Response.WriteAsJsonAsync(
            new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Status = StatusCodes.Status429TooManyRequests,
                Title = "Too many requests.",
                Extensions =
                {
                    ["correlationId"] = context.HttpContext.TraceIdentifier
                }
            },
            cancellationToken);
    };
    options.AddPolicy("mahak-api", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 120,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<MahakReadinessHealthCheck>("mahak-sql", tags: ["ready"]);

var app = builder.Build();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();
if (app.Environment.IsProduction())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

var api = app.MapGroup("/api/v1")
    .RequireAuthorization()
    .RequireRateLimiting("mahak-api");
api.MapItemEndpoint()
    .MapRepresentativeEndpoint()
    .MapInvoiceEndpoint()
    .MapSalePriceEndpoint();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("live")
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready")
});

app.Run();

public partial class Program;
