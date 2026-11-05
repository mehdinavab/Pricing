using Kahrad.Pricing.Products.Domain.Enums;
namespace Kahrad.Pricing.Products.Application.DTOs.Catalog;
public sealed record LanguageDto(long Id, string Code, string Name, bool IsActive, bool IsDefault, byte[] RowVersion);
public sealed record TranslationDto(long EntityId, string LanguageCode, string Text, string? Description, byte[] RowVersion);
public sealed record DefinitionDto(long Id, string Code, string Name, SpecificationValueType? ValueType, string? Unit);
public sealed record MissingTranslationDto(long EntityId, string LegacyName);
public sealed class ProductSpecificationDto
{
    public long Id { get; init; }
    public long SpecificationId { get; init; }
    public string Name { get; init; } = null!;
    public string? Unit { get; init; }
    public SpecificationValueType ValueType { get; init; }
    public string? TextValue { get; init; }
    public decimal? NumberValue { get; init; }
    public bool? BooleanValue { get; init; }
    public DateTime? DateValue { get; init; }
    public string? CodeValue { get; init; }
}
public sealed record CatalogLanguage(long Id, string Code, long DefaultId, string DefaultCode);
public sealed record LanguagePreference(string Code, double Quality, int Order);

public sealed record SpecificationAssignmentDto(long Id, long ProductId, long SpecificationId, string? TextValue,
    decimal? NumberValue, bool? BooleanValue, DateTime? DateValue, string? CodeValue, int DisplayOrder, byte[] RowVersion);
