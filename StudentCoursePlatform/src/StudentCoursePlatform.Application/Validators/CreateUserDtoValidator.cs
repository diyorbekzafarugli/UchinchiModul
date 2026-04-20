using FluentValidation;
using Microsoft.Extensions.Localization;
using StudentCoursePlatform.Application.DTOs.Users.Requests;
using StudentCoursePlatform.Application.Resources;

namespace StudentCoursePlatform.Application.Validators;

public class CreateUserDtoValidator : AbstractValidator<UserRegisterDto>
{
    public CreateUserDtoValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.FullName)
            .NotNull().WithMessage(localizer["FullNameRequired"])
            .NotEmpty().WithMessage(localizer["FullNameRequired"])
            .MaximumLength(50).WithMessage(localizer["FullNameMaxLength"]);

        RuleFor(x => x.Email)
            .NotNull().WithMessage(localizer["EmailRequired"])
            .NotEmpty().WithMessage(localizer["EmailRequired"])
            .EmailAddress().WithMessage(localizer["EmailInvalidFormat"]);

        RuleFor(x => x.Password)
            .NotNull().WithMessage(localizer["PasswordRequired"])
            .NotEmpty().WithMessage(localizer["PasswordRequired"])
            .MinimumLength(8).WithMessage(localizer["PasswordMinLength"])
            .Must(p => !p.Contains(" ")).WithMessage(localizer["PasswordNoSpaces"])
            .Must(PasswordValidator.HasUppercase).WithMessage(localizer["PasswordUppercase"])
            .Must(PasswordValidator.HasLowercase).WithMessage(localizer["PasswordLowercase"])
            .Must(PasswordValidator.HasDigit).WithMessage(localizer["PasswordDigit"])
            .Must(PasswordValidator.HasSpecialChar).WithMessage(localizer["PasswordSpecialChar"]);
    }
}
