using FluentValidation;
using Kahrad.Pricing.Brands.Application.DTOs.Stores;
using Kahrad.Pricing.BuildingBlocks.Application.Validation.Extensions;

namespace Kahrad.Pricing.Brands.Application.Validation.Stores;

public sealed class UpdateStoreRequestValidator : AbstractValidator<UpdateStoreRequest>
{
    public UpdateStoreRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(200);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50)
            .Matches("^[a-zA-Z0-9_-]+$")
            .WithMessage("Code must contain only alphanumeric characters, underscores, or hyphens.");

        RuleFor(x => x.OwnerName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(200);

        RuleFor(x => x.OwnerPhoneNumber!)
            .ValidIranianMobileNumber()
            .When(x => !string.IsNullOrWhiteSpace(x.OwnerPhoneNumber));

        RuleFor(x => x.OwnerNationalCode!)
            .ValidIranianNationalCode()
            .When(x => !string.IsNullOrWhiteSpace(x.OwnerNationalCode));

        RuleFor(x => x.OwnerEmail)
            .EmailAddress()
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.OwnerEmail));

        RuleFor(x => x.Address)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(1000);

        RuleFor(x => x.City)
            .MaximumLength(100);

        RuleFor(x => x.Province)
            .MaximumLength(100);

        RuleFor(x => x.PostalCode)
            .MaximumLength(30);

        RuleFor(x => x.Phone)
            .MaximumLength(50);

        RuleFor(x => x.PictureUrl)
            .MaximumLength(1000);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
