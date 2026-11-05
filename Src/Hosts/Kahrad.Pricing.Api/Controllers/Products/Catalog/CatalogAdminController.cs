using System.Security.Claims;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.Catalog;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Kahrad.Pricing.Api.Controllers.Products.Catalog;
[ApiController]
[Route("api/products/admin")]
[Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.SuperAdmin)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status409Conflict)]
public sealed class CatalogAdminController(ICatalogService catalog, IAuditService audit) : ControllerBase
{
    private Guid? UserId => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    private Task Audit(string action, string? id, CancellationToken ct) => audit.LogAsync(action, "Catalog", id,
        null, UserId, HttpContext.Connection.RemoteIpAddress?.ToString(), HttpContext.TraceIdentifier, ct);
    [HttpPost("languages/list")]
    [EndpointDescription("Lists active and inactive catalog languages for authorized manual editors.")]
    [ProducesResponseType<IReadOnlyCollection<LanguageDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Languages(CancellationToken ct) => Ok(await catalog.GetLanguagesAsync(ct));
    [HttpPost("languages/upsert")]
    [EndpointDescription("Adds or edits a language. Supply RowVersion from list for edits. The default must remain active; switching defaults requires translations for every active product.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Language([FromBody] LanguageRequest request, CancellationToken ct)
    {
        await catalog.UpsertLanguageAsync(request, UserId, ct); await Audit("CatalogLanguageUpdated", request.Code, ct); return NoContent();
    }
    [HttpPost("products/translations/upsert")]
    [EndpointDescription("Manually supplies a products translation. RowVersion from list is required for updates. Default content remains synchronized with legacy readers. Only textual specification values can be translated.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpsertProduct([FromBody] TranslationRequest request, CancellationToken ct)
    {
        await catalog.UpsertTranslationAsync(TranslationTarget.Product, request, UserId, ct);
        await Audit("ProductTranslationUpdated", request.EntityId.ToString(), ct); return NoContent();
    }
    [HttpPost("products/translations/list")]
    [EndpointDescription("Lists all manually supplied translations, including inactive languages, for this entity.")]
    [ProducesResponseType<IReadOnlyCollection<TranslationDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListProduct([FromBody] TranslationListRequest request, CancellationToken ct) =>
        Ok(await catalog.GetTranslationsAsync(TranslationTarget.Product, request.EntityId, ct));
    [HttpPost("features/translations/upsert")]
    [EndpointDescription("Manually supplies a features translation. RowVersion from list is required for updates. Default content remains synchronized with legacy readers. Only textual specification values can be translated.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpsertFeature([FromBody] TranslationRequest request, CancellationToken ct)
    {
        await catalog.UpsertTranslationAsync(TranslationTarget.Feature, request, UserId, ct);
        await Audit("FeatureTranslationUpdated", request.EntityId.ToString(), ct); return NoContent();
    }
    [HttpPost("features/translations/list")]
    [EndpointDescription("Lists all manually supplied translations, including inactive languages, for this entity.")]
    [ProducesResponseType<IReadOnlyCollection<TranslationDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListFeature([FromBody] TranslationListRequest request, CancellationToken ct) =>
        Ok(await catalog.GetTranslationsAsync(TranslationTarget.Feature, request.EntityId, ct));
    [HttpPost("specifications/translations/upsert")]
    [EndpointDescription("Manually supplies a specifications translation. RowVersion from list is required for updates. Default content remains synchronized with legacy readers. Only textual specification values can be translated.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpsertSpecification([FromBody] TranslationRequest request, CancellationToken ct)
    {
        await catalog.UpsertTranslationAsync(TranslationTarget.Specification, request, UserId, ct);
        await Audit("SpecificationTranslationUpdated", request.EntityId.ToString(), ct); return NoContent();
    }
    [HttpPost("specifications/translations/list")]
    [EndpointDescription("Lists all manually supplied translations, including inactive languages, for this entity.")]
    [ProducesResponseType<IReadOnlyCollection<TranslationDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListSpecification([FromBody] TranslationListRequest request, CancellationToken ct) =>
        Ok(await catalog.GetTranslationsAsync(TranslationTarget.Specification, request.EntityId, ct));
    [HttpPost("product-specifications/translations/upsert")]
    [EndpointDescription("Manually supplies a product-specifications translation. RowVersion from list is required for updates. Default content remains synchronized with legacy readers. Only textual specification values can be translated.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpsertProductSpecification([FromBody] TranslationRequest request, CancellationToken ct)
    {
        await catalog.UpsertTranslationAsync(TranslationTarget.ProductSpecification, request, UserId, ct);
        await Audit("ProductSpecificationTranslationUpdated", request.EntityId.ToString(), ct); return NoContent();
    }
    [HttpPost("product-specifications/translations/list")]
    [EndpointDescription("Lists all manually supplied translations, including inactive languages, for this entity.")]
    [ProducesResponseType<IReadOnlyCollection<TranslationDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListProductSpecification([FromBody] TranslationListRequest request, CancellationToken ct) =>
        Ok(await catalog.GetTranslationsAsync(TranslationTarget.ProductSpecification, request.EntityId, ct));
    [HttpPost("features/create")]
    [EndpointDescription("Creates one reusable shared definition with its manually supplied default-language label.")]
    [ProducesResponseType<long>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateFeatures([FromBody] DefinitionRequest request, CancellationToken ct)
    {
        var id = await catalog.CreateDefinitionAsync(false, request, UserId, ct);
        await Audit("CatalogDefinitionCreated", id.ToString(), ct); return Ok(id);
    }
    [HttpPost("features/list")]
    [EndpointDescription("Lists reusable definitions with deterministic database pagination; PageSize is capped at 100.")]
    [ProducesResponseType<IReadOnlyCollection<DefinitionDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListFeatures([FromBody] DefinitionListRequest request, CancellationToken ct) =>
        Ok(await catalog.GetDefinitionsAsync(false, request, ct));
    [HttpPost("specifications/create")]
    [EndpointDescription("Creates one reusable shared definition with its manually supplied default-language label.")]
    [ProducesResponseType<long>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateSpecifications([FromBody] DefinitionRequest request, CancellationToken ct)
    {
        var id = await catalog.CreateDefinitionAsync(true, request, UserId, ct);
        await Audit("CatalogDefinitionCreated", id.ToString(), ct); return Ok(id);
    }
    [HttpPost("specifications/list")]
    [EndpointDescription("Lists reusable definitions with deterministic database pagination; PageSize is capped at 100.")]
    [ProducesResponseType<IReadOnlyCollection<DefinitionDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ListSpecifications([FromBody] DefinitionListRequest request, CancellationToken ct) =>
        Ok(await catalog.GetDefinitionsAsync(true, request, ct));
    [HttpPost("product-specifications/list")]
    [EndpointDescription("Lists a bounded page of product specification assignments and RowVersion values for manual editing.")]
    [ProducesResponseType<IReadOnlyCollection<SpecificationAssignmentDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Assignments([FromBody] SpecificationAssignmentsRequest request, CancellationToken ct) =>
        Ok(await catalog.GetAssignmentsAsync(request, ct));
    [HttpPost("product-specifications/upsert")]
    [EndpointDescription("Assigns a shared specification or edits its typed value. Supply exactly one value matching the definition type and RowVersion for an existing assignment. Text is manually supplied in the default language.")]
    [ProducesResponseType<long>(StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignSpecification([FromBody] SpecificationAssignmentRequest request, CancellationToken ct)
    {
        var id = await catalog.AssignSpecificationAsync(request, UserId, ct);
        await Audit("ProductSpecificationUpdated", id.ToString(), ct); return Ok(id);
    }
    [HttpPost("translations/missing")]
    [EndpointDescription("Returns a bounded page of entities missing a translation for the specified language; no content is generated.")]
    [ProducesResponseType<IReadOnlyCollection<MissingTranslationDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Missing([FromBody] MissingTranslationsRequest request, CancellationToken ct) =>
        Ok(await catalog.GetMissingAsync(request, ct));
}
