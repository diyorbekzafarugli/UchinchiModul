using FluentValidation;
using Microsoft.Extensions.Localization;
using StudentCoursePlatform.Application.DTOs.Users.Requests;

namespace StudentCoursePlatform.Application.Validators;

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator(IStringLocalizer<UserUpdateDtoValidator> localizer)
    {
        RuleFor(x => x.FullName)
            .NotNull().WithMessage(localizer["FullNameRequired"])
            .NotEmpty().WithMessage(localizer["FullNameRequired"])
            .MaximumLength(50).WithMessage(localizer["FullNameMaxLength"]);
    }
}
