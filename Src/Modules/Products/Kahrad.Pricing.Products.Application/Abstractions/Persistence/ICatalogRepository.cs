using Kahrad.Pricing.Products.Domain.Entities;
using Kahrad.Pricing.Products.Application.DTOs.Catalog;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
namespace Kahrad.Pricing.Products.Application.Abstractions.Persistence;
public interface ICatalogRepository
{
    Task<bool> HasDefaultTranslationAsync(long productId, CancellationToken cancellationToken);
    Task<Feature?> GetFeatureAsync(long featureId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Language>> GetLanguagesAsync(CancellationToken cancellationToken);
    Task UpsertLanguageAsync(LanguageRequest request, Guid? userId, CancellationToken cancellationToken);
    Task UpsertTranslationAsync(TranslationTarget target, TranslationRequest request, Guid? userId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TranslationDto>> GetTranslationsAsync(TranslationTarget target, long entityId, CancellationToken cancellationToken);
    Task<long> CreateDefinitionAsync(bool specification, DefinitionRequest request, Guid? userId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<DefinitionDto>> GetDefinitionsAsync(bool specification, DefinitionListRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<SpecificationAssignmentDto>> GetAssignmentsAsync(SpecificationAssignmentsRequest request, CancellationToken cancellationToken);
    Task<long> AssignSpecificationAsync(SpecificationAssignmentRequest request, Guid? userId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<MissingTranslationDto>> GetMissingAsync(MissingTranslationsRequest request, CancellationToken cancellationToken);
    Task<BaseResponsePaginated<ProductDetailDto>> SearchAsync(CatalogSearchRequest request, CatalogLanguage language, CancellationToken cancellationToken);
    Task<ProductDetailDto?> GetDetailsAsync(long productId, CatalogLanguage language, CancellationToken cancellationToken);
}
