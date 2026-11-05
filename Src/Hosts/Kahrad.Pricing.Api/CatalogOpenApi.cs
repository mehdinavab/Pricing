using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace Kahrad.Pricing.Api;
public sealed class CatalogSwaggerOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.ApiDescription.RelativePath?.StartsWith("api/products/", StringComparison.OrdinalIgnoreCase) != true) return;
        if (context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any())
            operation.Security = [];
    }
}
public static class CatalogOpenApi
{
    public static void AddCatalogSecurity(OpenApiDocument document)
    {
        if (document.Paths is null) return;
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["CatalogBearer"] = new OpenApiSecurityScheme
        { Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT", Description = "Catalog admin actions require an Admin or SuperAdmin JWT." };
        foreach (var path in document.Paths.Where(x => x.Key.StartsWith("/api/products/", StringComparison.OrdinalIgnoreCase)))
        {
            if (path.Value.Operations is null) continue;
            foreach (var operation in path.Value.Operations.Values)
            {
                if (path.Key.StartsWith("/api/products/admin/", StringComparison.OrdinalIgnoreCase))
                    operation.Security = [new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference("CatalogBearer", document)] = [] }];
                else if (path.Key is "/api/products/search" or "/api/products/details") operation.Security = [];
            }
        }
    }
}
