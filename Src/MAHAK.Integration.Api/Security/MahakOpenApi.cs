using Microsoft.OpenApi;

namespace MAHAK.Integration.Api.Security;

public static class MahakOpenApi
{
    public static void AddApiKeySecurity(OpenApiDocument document)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[ApiKeyAuthenticationHandler.SchemeName] =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = ApiKeyAuthenticationHandler.HeaderName,
                In = ParameterLocation.Header,
                Description = "Brand-specific MAHAK integration API key."
            };

        if (document.Paths is null)
            return;
        foreach (var path in document.Paths.Where(path =>
                     path.Key.StartsWith("/api/v1/", StringComparison.OrdinalIgnoreCase)))
        {
            if (path.Value.Operations is null)
                continue;
            foreach (var operation in path.Value.Operations.Values)
            {
                operation.Security =
                [
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference(
                            ApiKeyAuthenticationHandler.SchemeName,
                            document)] = []
                    }
                ];
            }
        }
    }
}
