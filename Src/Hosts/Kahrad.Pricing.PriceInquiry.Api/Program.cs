using Kahrad.Pricing.BuildingBlocks.Infrastructure.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostLogging(builder.Configuration, builder.Environment.ApplicationName);
builder.Services.AddOpenApi();
builder.Services.AddApiHealth();
// Fail readiness until published-price persistence and delivery are implemented. Never advertise demo prices.
builder.Services.AddHealthChecks().AddCheck("price-delivery", () =>
    HealthCheckResult.Unhealthy("Published-price delivery has not been configured."), tags: ["ready"]);
var app = builder.Build();
app.UseHostRequestLogging();
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.UseHttpsRedirection();
app.MapHostHealth();
app.Run();
