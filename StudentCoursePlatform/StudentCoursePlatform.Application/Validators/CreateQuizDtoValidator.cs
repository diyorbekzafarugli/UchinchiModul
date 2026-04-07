using FluentValidation;
using Microsoft.Extensions.Localization;
using StudentCoursePlatform.Application.DTOs.Quizzes.Requests;
using StudentCoursePlatform.Application.Resources;

namespace StudentCoursePlatform.Application.Validators;

public class CreateQuizDtoValidator : AbstractValidator<CreateQuizDto>
{
    public CreateQuizDtoValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage(localizer["CourseIdRequired"]);

        RuleFor(x => x.Title)
            .NotNull().WithMessage(localizer["TitleRequired"])
            .NotEmpty().WithMessage(localizer["TitleRequired"])
            .MaximumLength(200).WithMessage(localizer["TitleMaxLength"]);

        RuleFor(x => x.TimeLimitMinutes)
            .GreaterThan(0).WithMessage(localizer["TimeLimitPositive"]);

        RuleFor(x => x.PassScore)
            .GreaterThanOrEqualTo(0).WithMessage(localizer["PassScoreNonNegative"]);
    }
}