using Kahrad.Pricing.Api.Controllers.Products.Catalog;
using Kahrad.Pricing.Products.Application.Services;
using Kahrad.Pricing.Products.Application.Validation.Catalog;
using Kahrad.Pricing.Products.Application.DTOs.Catalog;
using Kahrad.Pricing.Products.Domain.Entities;
using Kahrad.Pricing.Products.Domain.Enums;
using Kahrad.Pricing.Products.Domain.Exceptions;
using Xunit;
namespace Kahrad.Pricing.Products.Tests;
public sealed class CatalogUnitTests
{
    private static Language[] Languages() => [new("fa", "فارسی", true, true), new("ar", "العربية"), new("en", "English")];
    [Theory]
    [InlineData("fa", "fa")]
    [InlineData("ar", "ar")]
    [InlineData("en", "en")]
    [InlineData("en-US", "en")]
    [InlineData("ar;q=0.4,en;q=0.9,fa;q=0.7", "en")]
    [InlineData("ar;q=0,en;q=0.5", "en")]
    [InlineData("en;q=0,en-US;q=1,ar;q=0.5", "ar")]
    [InlineData("de,ar;q=0.8", "ar")]
    [InlineData("*", "fa")]
    [InlineData("fa;q=0,ar;q=0,en;q=0", "fa")]
    [InlineData("", "fa")]
    public void Resolves_quality_neutral_and_excluded_locales(string header, string expected)
    {
        var result = new LanguageResolver().Resolve(Languages(), CatalogController.ParseLanguages(header));
        Assert.Equal(expected, result.Code);
    }
    [Fact]
    public void Inactive_language_is_not_selected()
    {
        var languages = Languages(); languages[1].Configure("Arabic", false, false);
        Assert.Equal("en", new LanguageResolver().Resolve(languages, CatalogController.ParseLanguages("ar,en;q=0.5")).Code);
    }
    [Fact]
    public void Exact_regional_locale_precedes_neutral()
    {
        var languages = Languages().Append(new Language("en-US", "US English")).ToArray();
        Assert.Equal("en-US", new LanguageResolver().Resolve(languages, CatalogController.ParseLanguages("en-US")).Code);
    }
    [Fact]
    public void Default_cannot_be_disabled_or_removed()
    {
        var fallback = Languages()[0];
        Assert.Throws<CatalogRuleException>(() => fallback.Configure("Persian", false, true));
        Assert.Throws<CatalogRuleException>(() => fallback.Configure("Persian", true, false));
    }
    [Theory]
    [InlineData("en-us", "en-US")]
    [InlineData("FA", "fa")]
    public void Normalizes_language_codes(string input, string expected) => Assert.Equal(expected, Language.NormalizeCode(input));
    [Fact]
    public void Typed_values_reject_translation_or_conversion_to_text()
    {
        var value = new ProductSpecification(1, 1);
        value.SetValue(SpecificationValueType.Number, null, 12.5m, null, null, null, 0);
        Assert.Equal(12.5m, value.NumberValue); Assert.Null(value.TextValue);
        Assert.Throws<CatalogRuleException>(() => value.SetValue(SpecificationValueType.Number, "12.5", null, null, null, null, 0));
        value.SetValue(SpecificationValueType.Boolean, null, null, false, null, null, 0);
        Assert.False(value.BooleanValue); Assert.Null(value.TextValue);
    }
    [Fact]
    public void Typed_storage_does_not_silently_round_or_truncate_values()
    {
        var value = new ProductSpecification(1, 1);
        Assert.Throws<CatalogRuleException>(() => value.SetValue(SpecificationValueType.Number, null, 1.12345m, null, null, null, 0));
        Assert.Throws<CatalogRuleException>(() => value.SetValue(SpecificationValueType.Date, null, null, null, new DateTime(2026,11,5,12,0,0), null, 0));
    }
    [Fact]
    public void Existing_feature_assignment_accepts_shared_id_without_a_duplicate_name()
    {
        var validator = new Kahrad.Pricing.Products.Application.Validation.Features.AddProductFeatureRequestValidator();
        Assert.True(validator.Validate(new Kahrad.Pricing.Products.Application.DTOs.Features.AddProductFeatureRequest { FeatureId = 1, Value = "yes" }).IsValid);
        Assert.False(validator.Validate(new Kahrad.Pricing.Products.Application.DTOs.Features.AddProductFeatureRequest { Value = "yes" }).IsValid);
    }
    [Fact]
    public void Application_validation_limits_page_size_and_empty_text()
    {
        Assert.False(new CatalogSearchValidator().Validate(new CatalogSearchRequest { PageSize = 101 }).IsValid);
        Assert.False(new TranslationRequestValidator().Validate(new TranslationRequest(1, "en", " ")).IsValid);
    }
}
