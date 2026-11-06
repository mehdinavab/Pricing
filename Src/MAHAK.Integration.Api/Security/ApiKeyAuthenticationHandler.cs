using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MAHAK.Integration.Api.Security;

public sealed class ApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IApiKeyBrandResolver resolver)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "MahakApiKey";
    public const string HeaderName = "X-API-Key";
    public static readonly object BrandContextItemKey = new();

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(HeaderName, out var values) || values.Count != 1)
            return Task.FromResult(AuthenticateResult.Fail("A single API key is required."));

        var apiKey = values[0];
        if (string.IsNullOrWhiteSpace(apiKey) || !resolver.TryResolve(apiKey, out var resolution))
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));

        Context.Items[BrandContextItemKey] = resolution!.Brand;
        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, resolution.KeyId),
                new Claim("mahak_brand", resolution.Brand.BrandCode)
            ],
            SchemeName);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = "application/problem+json";
        await JsonSerializer.SerializeAsync(
            Response.Body,
            new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Authentication is required.",
                Extensions = { ["correlationId"] = Context.TraceIdentifier }
            },
            cancellationToken: Context.RequestAborted);
    }
}
