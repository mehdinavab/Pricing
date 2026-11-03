using Kahrad.Pricing.Api;
using Kahrad.Pricing.Identity.Application;
using Kahrad.Pricing.Identity.Infrastructure;
using Kahrad.Pricing.Identity.Infrastructure.Services.models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
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
builder.Services.AddControllers(options =>
    options.Filters.AddService<FluentValidationActionFilter>());
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
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
builder.Services.AddExceptionHandler<IdentityExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);
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

app.UseExceptionHandler();

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

app.Run();
