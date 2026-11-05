using FluentValidation;
using Kahrad.Pricing.Products.Application.Abstractions.Persistence;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.Catalog;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Kahrad.Pricing.Products.Application.Exceptions;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
namespace Kahrad.Pricing.Products.Application.Services;
public sealed class CatalogService(ICatalogRepository repository, ILanguageResolver resolver,
    IValidator<CatalogSearchRequest> searchValidator, IValidator<LanguageRequest> languageValidator,
    IValidator<TranslationRequest> translationValidator, IValidator<DefinitionRequest> definitionValidator,
    IValidator<SpecificationAssignmentRequest> assignmentValidator) : ICatalogService
{
    public async Task<CatalogLanguage> ResolveLanguageAsync(IReadOnlyCollection<LanguagePreference> preferences, CancellationToken ct) =>
        resolver.Resolve(await repository.GetLanguagesAsync(ct), preferences);

    private static async Task Validate<T>(T request, IValidator<T> validator, CancellationToken ct)
    {
        var result = await validator.ValidateAsync(request, ct);
        if (!result.IsValid) throw new ProductOperationException(string.Join(" ", result.Errors.Select(x => x.ErrorMessage)));
    }
    public async Task<BaseResponsePaginated<ProductDetailDto>> SearchAsync(CatalogSearchRequest request, IReadOnlyCollection<LanguagePreference> preferences, CancellationToken ct)
    {
        await Validate(request, searchValidator, ct);
        return await repository.SearchAsync(request, resolver.Resolve(await repository.GetLanguagesAsync(ct), preferences), ct);
    }
    public async Task<ProductDetailDto?> GetDetailsAsync(long id, IReadOnlyCollection<LanguagePreference> preferences, CancellationToken ct)
    {
        if (id <= 0) throw new ProductOperationException("Product ID must be positive.");
        return await repository.GetDetailsAsync(id, resolver.Resolve(await repository.GetLanguagesAsync(ct), preferences), ct);
    }
    public async Task<IReadOnlyCollection<LanguageDto>> GetLanguagesAsync(CancellationToken ct) =>
        (await repository.GetLanguagesAsync(ct)).Select(x => new LanguageDto(x.Id, x.Code, x.Name, x.IsActive, x.IsDefault, x.RowVersion)).ToArray();
    public async Task UpsertLanguageAsync(LanguageRequest request, Guid? userId, CancellationToken ct)
    {
        await Validate(request, languageValidator, ct); await repository.UpsertLanguageAsync(request, userId, ct);
    }
    public async Task UpsertTranslationAsync(TranslationTarget target, TranslationRequest request, Guid? userId, CancellationToken ct)
    {
        await Validate(request, translationValidator, ct);
        if (!Enum.IsDefined(target)) throw new ProductOperationException("Invalid translation target.");
        if (target != TranslationTarget.Product && request.Description is not null) throw new ProductOperationException("Only product descriptions are localized.");
        await repository.UpsertTranslationAsync(target, request, userId, ct);
    }
    public Task<IReadOnlyCollection<TranslationDto>> GetTranslationsAsync(TranslationTarget target, long id, CancellationToken ct) => repository.GetTranslationsAsync(target, id, ct);
    public async Task<long> CreateDefinitionAsync(bool specification, DefinitionRequest request, Guid? userId, CancellationToken ct)
    {
        await Validate(request, definitionValidator, ct); return await repository.CreateDefinitionAsync(specification, request, userId, ct);
    }
    public Task<IReadOnlyCollection<DefinitionDto>> GetDefinitionsAsync(bool specification, DefinitionListRequest request, CancellationToken ct)
    {
        CheckPage(request.Page, request.PageSize); return repository.GetDefinitionsAsync(specification, request, ct);
    }
    public Task<IReadOnlyCollection<SpecificationAssignmentDto>> GetAssignmentsAsync(SpecificationAssignmentsRequest request, CancellationToken ct)
    {
        CheckPage(request.Page, request.PageSize);
        if (request.ProductId <= 0) throw new ProductOperationException("Product ID must be positive.");
        return repository.GetAssignmentsAsync(request, ct);
    }
    public async Task<long> AssignSpecificationAsync(SpecificationAssignmentRequest request, Guid? userId, CancellationToken ct)
    {
        await Validate(request, assignmentValidator, ct); return await repository.AssignSpecificationAsync(request, userId, ct);
    }
    public Task<IReadOnlyCollection<MissingTranslationDto>> GetMissingAsync(MissingTranslationsRequest request, CancellationToken ct)
    {
        CheckPage(request.Page, request.PageSize); return repository.GetMissingAsync(request, ct);
    }
    private static void CheckPage(int page, int size)
    {
        if (page < 1 || page > 1000000 || size is < 1 or > 100) throw new ProductOperationException("Invalid catalog pagination.");
    }
}
