using Kahrad.Pricing.Products.Domain.Enums;
namespace Kahrad.Pricing.Products.Application.DTOs.Catalog;
public enum TranslationTarget { Product = 1, Feature = 2, Specification = 3, ProductSpecification = 4 }
public sealed record EntityRequest(long EntityId);
public sealed record CatalogDetailsRequest(long ProductId);
public sealed class CatalogSearchRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public long? BrandId { get; init; }
    public long? GroupId { get; init; }
    public string? Name { get; init; }
    public string? Code { get; init; }
    public bool SortByName { get; init; }
}
public sealed record LanguageRequest(string Code, string Name, bool IsActive = true, bool IsDefault = false, byte[]? RowVersion = null);
public sealed record TranslationRequest(long EntityId, string LanguageCode, string Text, string? Description = null, byte[]? RowVersion = null);
public sealed record TranslationListRequest(long EntityId);
public sealed record DefinitionRequest(string Code, string Name, SpecificationValueType ValueType = SpecificationValueType.Text, string? Unit = null);
public sealed record DefinitionListRequest(int Page = 1, int PageSize = 20);
public sealed record MissingTranslationsRequest(TranslationTarget Target, string LanguageCode, int Page = 1, int PageSize = 20);
public sealed record SpecificationAssignmentRequest(long ProductId, long SpecificationId, string? TextValue = null,
    decimal? NumberValue = null, bool? BooleanValue = null, DateTime? DateValue = null, string? CodeValue = null,
    int DisplayOrder = 0, byte[]? RowVersion = null);

public sealed record SpecificationAssignmentsRequest(long ProductId, int Page = 1, int PageSize = 20);
