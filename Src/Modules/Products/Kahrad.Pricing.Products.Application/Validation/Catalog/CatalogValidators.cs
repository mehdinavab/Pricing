using FluentValidation;
using Kahrad.Pricing.Products.Application.DTOs.Catalog;
namespace Kahrad.Pricing.Products.Application.Validation.Catalog;
public sealed class CatalogSearchValidator : AbstractValidator<CatalogSearchRequest>
{
    public CatalogSearchValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).LessThanOrEqualTo(1000000);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.BrandId).GreaterThan(0).When(x => x.BrandId.HasValue);
        RuleFor(x => x.GroupId).GreaterThan(0).When(x => x.GroupId.HasValue);
        RuleFor(x => x.Name).MaximumLength(250); RuleFor(x => x.Code).MaximumLength(100);
    }
}
public sealed class LanguageRequestValidator : AbstractValidator<LanguageRequest>
{
    public LanguageRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(16);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.IsActive).Equal(true).When(x => x.IsDefault);
    }
}
public sealed class TranslationRequestValidator : AbstractValidator<TranslationRequest>
{
    public TranslationRequestValidator()
    {
        RuleFor(x => x.EntityId).GreaterThan(0); RuleFor(x => x.LanguageCode).NotEmpty().MaximumLength(16);
        RuleFor(x => x.Text).NotEmpty().MaximumLength(500); RuleFor(x => x.Description).MaximumLength(1000);
    }
}
public sealed class DefinitionRequestValidator : AbstractValidator<DefinitionRequest>
{
    public DefinitionRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100); RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.ValueType).IsInEnum(); RuleFor(x => x.Unit).MaximumLength(50);
    }
}
public sealed class SpecificationAssignmentValidator : AbstractValidator<SpecificationAssignmentRequest>
{
    public SpecificationAssignmentValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0); RuleFor(x => x.SpecificationId).GreaterThan(0);
        RuleFor(x => x.TextValue).MaximumLength(500); RuleFor(x => x.CodeValue).MaximumLength(100);
    }
}

public sealed class TranslationListValidator : AbstractValidator<TranslationListRequest>
{
    public TranslationListValidator() { RuleFor(x => x.EntityId).GreaterThan(0); }
}
public sealed class CatalogDetailsValidator : AbstractValidator<CatalogDetailsRequest>
{
    public CatalogDetailsValidator() { RuleFor(x => x.ProductId).GreaterThan(0); }
}
public sealed class MissingTranslationsValidator : AbstractValidator<MissingTranslationsRequest>
{
    public MissingTranslationsValidator()
    {
        RuleFor(x => x.Target).IsInEnum(); RuleFor(x => x.LanguageCode).NotEmpty().MaximumLength(16);
        RuleFor(x => x.Page).InclusiveBetween(1, 1000000); RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
public sealed class DefinitionListValidator : AbstractValidator<DefinitionListRequest>
{
    public DefinitionListValidator() { RuleFor(x => x.Page).InclusiveBetween(1, 1000000); RuleFor(x => x.PageSize).InclusiveBetween(1, 100); }
}

public sealed class SpecificationAssignmentsValidator : AbstractValidator<SpecificationAssignmentsRequest>
{
    public SpecificationAssignmentsValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0); RuleFor(x => x.Page).InclusiveBetween(1, 1000000);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
