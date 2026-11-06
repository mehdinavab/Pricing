using Kahrad.Pricing.BuildingBlocks.Infrastructure.Hosting;
using Kahrad.Pricing.Api;
using Kahrad.Pricing.Brands.Application;
using Kahrad.Pricing.Brands.Infrastructure;
using Kahrad.Pricing.Guarantees.Application;
using Kahrad.Pricing.Guarantees.Infrastructure;
using Kahrad.Pricing.Identity.Application;
using Kahrad.Pricing.Identity.Infrastructure;
using Kahrad.Pricing.Identity.Infrastructure.Services.models;
using Kahrad.Pricing.Products.Application;
using Kahrad.Pricing.Products.Infrastructure;
using Kahrad.Pricing.Sms.Application;
using Kahrad.Pricing.Sms.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Kahrad.Pricing.Identity.Infrastructure.Persistence;
using Kahrad.Pricing.Identity.Infrastructure.Persistence.Seed;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text;
using System.Globalization;
using System.Threading.RateLimiting;
using Kahrad.Pricing.Api.Localization;
using Kahrad.Pricing.Identity.Application.Authentication;
using Kahrad.Pricing.AccountingIntegration.Application;
using Kahrad.Pricing.AccountingIntegration.Infrastructure;
using Kahrad.Pricing.Sales.Application;
using Kahrad.Pricing.Sales.Infrastructure;
using Kahrad.Pricing.Representatives.Application;
using Kahrad.Pricing.Representatives.Infrastructure;
using Kahrad.Pricing.Pricing.Application;
using Kahrad.Pricing.Pricing.Infrastructure;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostLogging(builder.Configuration, builder.Environment.ApplicationName);
builder.Services.AddApiHealth();
var healthConnections = new[] { "IdentityDb", "BrandsDb", "ProductsDb", "GuaranteesDb", "SmsDb" }
    .Select(name => builder.Configuration.GetConnectionString(name) ?? builder.Configuration.GetConnectionString("DefaultConnection"))
    .Distinct().ToArray();
for (var index = 0; index < healthConnections.Length; index++)
    builder.Services.AddRequiredSqlReadiness($"sql-{index}", healthConnections[index]);
var jwtOptions = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtOptions>() ?? new JwtOptions();

if (string.IsNullOrWhiteSpace(jwtOptions.Key) ||
    string.IsNullOrWhiteSpace(jwtOptions.Issuer) ||
    string.IsNullOrWhiteSpace(jwtOptions.Audience) ||
    Encoding.UTF8.GetByteCount(jwtOptions.Key) < 32 ||
    jwtOptions.AccessTokenLifetimeMinutes <= 0 ||
    jwtOptions.RefreshTokenLifetimeDays <= 0)
{
    throw new InvalidOperationException(
        "JWT settings must include a 32-byte key, issuer, audience, and positive token lifetimes.");
}

builder.Services.AddScoped<FluentValidationActionFilter>();
var otpOptions = builder.Configuration
    .GetSection(OtpAuthenticationOptions.SectionName)
    .Get<OtpAuthenticationOptions>() ?? new OtpAuthenticationOptions();
otpOptions.Validate();
builder.Services.Configure<OtpAuthenticationOptions>(
    builder.Configuration.GetSection(OtpAuthenticationOptions.SectionName));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("otp-auth", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});
builder.Services.AddLocalization();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = [new CultureInfo("en-US"), new CultureInfo("fa-IR"), new CultureInfo("ar-SA")];
    options.SupportedUICultures = options.SupportedCultures;
    options.RequestCultureProviders = [new ApiRequestCultureProvider()];
});
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
    options.InvalidModelStateResponseFactory = context =>
    {
        var messages = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<ApiMessages>>();
        var errors = context.ModelState.Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(x => x.Key, x => x.Value!.Errors
                .Select(error => ApiText.Translate(messages, error.ErrorMessage, "The request is invalid."))
                .ToArray());
        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(
            new Microsoft.AspNetCore.Mvc.ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = messages["The request is invalid."]
            });
    });
builder.Services.AddScoped<Kahrad.Pricing.Api.Authorization.ProtectSuperAdminFilter>();
builder.Services.AddControllers(options =>
    options.Filters.AddService<FluentValidationActionFilter>());
builder.Services.AddOpenApi(options => options.AddDocumentTransformer((document, context, cancellationToken) =>
{
    CatalogOpenApi.AddCatalogSecurity(document);
    return Task.CompletedTask;
}));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<CatalogSwaggerOperationFilter>();
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Kahrad Pricing API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter a JWT access token."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
{
    var messages = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<ApiMessages>>();
    if (context.ProblemDetails.Status == StatusCodes.Status500InternalServerError)
    {
        context.ProblemDetails.Title = messages["The server encountered an error."];
        context.ProblemDetails.Detail = null;
    }
});
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddBrandsApplication();
builder.Services.AddBrandsInfrastructure(builder.Configuration);
builder.Services.AddGuaranteesApplication();
builder.Services.AddGuaranteesInfrastructure(builder.Configuration);
builder.Services.AddProductsApplication();
builder.Services.AddProductsInfrastructure(builder.Configuration);
builder.Services.AddSmsApplication();
builder.Services.AddSmsInfrastructure(builder.Configuration);
builder.Services.AddAccountingApplication();
builder.Services.AddAccountingInfrastructure(builder.Configuration);
builder.Services.AddSalesApplication();
builder.Services.AddSalesInfrastructure(builder.Configuration);
builder.Services.AddRepresentativesApplication();
builder.Services.AddRepresentativesInfrastructure(builder.Configuration);
builder.Services.AddPricingApplication();
builder.Services.AddPricingInfrastructure(builder.Configuration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (args.Contains("--seed-superadmin", StringComparer.OrdinalIgnoreCase))
{
    if (!app.Environment.IsDevelopment())
        throw new InvalidOperationException("Development superadmin seeding requires the Development environment.");
    var seedUsername = builder.Configuration["IdentitySeed:Username"];
    var seedPassword = builder.Configuration["IdentitySeed:Password"];
    if (string.IsNullOrWhiteSpace(seedUsername) || string.IsNullOrWhiteSpace(seedPassword))
        throw new InvalidOperationException("Set IdentitySeed:Username and IdentitySeed:Password through secure configuration.");
    await using var scope = app.Services.CreateAsyncScope();
    var identityContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    await identityContext.Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<IdentitySeeder>()
        .SeedDevelopmentSuperAdminAsync(seedUsername, seedPassword);
    app.Logger.LogInformation("Development SuperAdmin seed verified for {Username}.", seedUsername);
    return;
}


app.UseHostRequestLogging();
app.UseRequestLocalization();
app.Use(async (context, next) =>
{
    context.Response.Headers.ContentLanguage = CultureInfo.CurrentUICulture.Name;
    context.Response.Headers.Append("Vary", "Accept-Language, X-Region");
    await next();
});
app.UseExceptionHandler();
app.UseMiddleware<ApiOperationLoggingMiddleware>();
app.UseRouting();
app.UseRateLimiter();
app.UseStatusCodePages(async statusContext =>
{
    var context = statusContext.HttpContext;
    var messages = context.RequestServices.GetRequiredService<IStringLocalizer<ApiMessages>>();
    var key = context.Response.StatusCode switch
    {
        StatusCodes.Status401Unauthorized => "Unauthorized",
        StatusCodes.Status403Forbidden => "Forbidden",
        StatusCodes.Status404NotFound => "Resource not found",
        _ => "The request is invalid."
    };
    await context.Response.WriteAsJsonAsync(new Microsoft.AspNetCore.Mvc.ProblemDetails
    {
        Status = context.Response.StatusCode,
        Title = messages[key]
    });
});

// Configure the HTTP request pipeline.
var configuredUrls = builder.WebHost.GetSetting("urls");
var hasHttpsEndpoint = configuredUrls?
    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Any(url => Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
    == true;

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Kahrad Pricing API v1");
        options.RoutePrefix = "swagger";
    });
    app.MapScalarApiReference(options =>
        options.WithTitle("Kahrad Pricing API"));
}

if (!app.Environment.IsDevelopment() || hasHttpsEndpoint)
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHostHealth();

app.Run();

public partial class Program { }
