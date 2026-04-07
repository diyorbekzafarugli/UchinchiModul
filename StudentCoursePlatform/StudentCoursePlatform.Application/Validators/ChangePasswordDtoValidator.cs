using FluentValidation;
using Microsoft.Extensions.Localization;
using StudentCoursePlatform.Application.DTOs.Users.Requests;
using StudentCoursePlatform.Application.Resources;

namespace StudentCoursePlatform.Application.Validators;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.NewPassword)
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
