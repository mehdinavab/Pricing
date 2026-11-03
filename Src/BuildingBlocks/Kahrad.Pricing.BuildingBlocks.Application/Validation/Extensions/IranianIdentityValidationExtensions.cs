using FluentValidation;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kahrad.Pricing.BuildingBlocks.Application.Validation.Extensions
{
    public static class IranianIdentityValidationExtensions
    {
        private const string IranRegionCode = "IR";
        private const int ShebaLength = 24;
        private static readonly PhoneNumberUtil PhoneUtil = PhoneNumberUtil.GetInstance();

        public static IRuleBuilderOptions<T, string> ValidIranianNationalCode<T>(
            this IRuleBuilderInitial<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("NationalCodeIsRequiredError")
                .Must(IsValidIranianNationalCode)
                    .WithMessage("InvalidNationalCodeError");
        }

        public static IRuleBuilderOptions<T, string> ValidIranianMobileNumber<T>(
            this IRuleBuilderInitial<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("PhoneNumberIsRequiredError")
                .Must(IsValidIranianMobileNumber)
                    .WithMessage("InvalidPhoneNumberError");
        }

        public static IRuleBuilderOptions<T, string?> ValidIranianSheba<T>(
            this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .Must(shaba =>
                    string.IsNullOrWhiteSpace(shaba) ||
                    IsValidIranianSheba(
                        $"IR{shaba.Trim()}"))
                .WithMessage("InvalidIranianShabaError");
        }

        public static DateTime? NormalizeBirthDate(this DateTime? birthDate)
        {
            return birthDate is null
                ? null
                : DateTime.SpecifyKind(birthDate.Value.Date, DateTimeKind.Utc);
        }

        internal static bool IsValidIranianNationalCode(string? nationalCode)
        {
            if (nationalCode is null ||
                nationalCode.Length != 10 ||
                nationalCode.Any(character => !char.IsAsciiDigit(character)) ||
                nationalCode.All(character => character == nationalCode[0]))
            {
                return false;
            }

            int sum = 0;
            for (int index = 0; index < 9; index++)
            {
                sum += (nationalCode[index] - '0') * (10 - index);
            }

            int remainder = sum % 11;
            int expectedCheckDigit = remainder < 2 ? remainder : 11 - remainder;
            return nationalCode[9] - '0' == expectedCheckDigit;
        }

        internal static bool IsValidIranianMobileNumber(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return false;
            }

            try
            {
                PhoneNumber parsedPhoneNumber = PhoneUtil.Parse(phoneNumber, IranRegionCode);

                return PhoneUtil.IsValidNumberForRegion(parsedPhoneNumber, IranRegionCode) &&
                       PhoneUtil.GetNumberType(parsedPhoneNumber) == PhoneNumberType.MOBILE;
            }
            catch (NumberParseException)
            {
                return false;
            }
        }
        internal static bool IsValidIranianSheba(string? sheba)
        {
            if (string.IsNullOrWhiteSpace(sheba) ||
                sheba.Length != ShebaLength ||
                !sheba.StartsWith(IranRegionCode, StringComparison.Ordinal) ||
                sheba.Skip(2).Any(character => !char.IsAsciiDigit(character)))
            {
                return false;
            }
            string rearranged = string.Concat(sheba.AsSpan(4), sheba.AsSpan(0, 4));

            int remainder = 0;
            foreach (char character in rearranged)
            {
                remainder = char.IsAsciiDigit(character)
                    ? (remainder * 10 + (character - '0')) % 97
                    : (remainder * 100 + (character - 'A' + 10)) % 97;
            }

            return remainder == 1;
        }
    }
}
