using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.Catalog;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
namespace Kahrad.Pricing.Api.Controllers.Products.Catalog;
[ApiController]
[Route("api/products")]
[AllowAnonymous]
public sealed class CatalogController(ICatalogService catalog) : ControllerBase
{
    public static IReadOnlyCollection<LanguagePreference> ParseLanguages(string? header, string? region = null)
    {
        if (string.IsNullOrWhiteSpace(header) || header.Length > 4096 ||
            !StringWithQualityHeaderValue.TryParseList([header], out var values))
        {
            var code = region?.Trim().ToUpperInvariant() switch
            {
                "IR" => "fa",
                "SA" => "ar",
                "US" => "en",
                _ => null
            };
            return code is null ? [] : [new LanguagePreference(code, 1, 0)];
        }
        return values.Select((x, index) => new LanguagePreference(x.Value.ToString(), x.Quality ?? 1, index)).ToArray();
    }
    private void LanguageHeaders(string language)
    {
        Response.Headers.ContentLanguage = language;
        Response.Headers.Append("Vary", "Accept-Language");
    }
    [HttpPost("search")]
    [EndpointDescription("Searches active products using Accept-Language quality weights, exact/neutral matches, and per-field default-language fallback. Persian is the initial default. Returns one localized representation; no translations are generated. Vary: Accept-Language. The server returns its default if all supported languages are excluded.")]
    [ProducesResponseType<BaseResponsePaginated<ProductDetailDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResponsePaginated<ProductDetailDto>>> Search([FromBody] CatalogSearchRequest request,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage, CancellationToken cancellationToken)
    {
        var preferences = ParseLanguages(acceptLanguage, Request.Headers["X-Region"]);
        var result = await catalog.SearchAsync(request, preferences, cancellationToken);
        LanguageHeaders(result.Data.FirstOrDefault()?.Language ?? (await catalog.ResolveLanguageAsync(preferences, cancellationToken)).Code);
        return Ok(result);
    }
    [HttpPost("details")]
    [EndpointDescription("Returns an active product localized using Accept-Language. Each missing label/value falls back to the active default language, then preserved legacy content. Returns Content-Language and Vary: Accept-Language.")]
    [ProducesResponseType<ProductDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDetailDto>> Details([FromBody] CatalogDetailsRequest request,
        [FromHeader(Name = "Accept-Language")] string? acceptLanguage, CancellationToken cancellationToken)
    {
        var result = await catalog.GetDetailsAsync(request.ProductId, ParseLanguages(acceptLanguage, Request.Headers["X-Region"]), cancellationToken);
        if (result is null) return NotFound();
        LanguageHeaders(result.Language!); return Ok(result);
    }
}
