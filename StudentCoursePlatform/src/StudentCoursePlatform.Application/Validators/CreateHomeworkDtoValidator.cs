using FluentValidation;
using Microsoft.Extensions.Localization;
using StudentCoursePlatform.Application.DTOs.Homeworks.Requests;
using StudentCoursePlatform.Application.Resources;

namespace StudentCoursePlatform.Application.Validators;

public class CreateHomeworkDtoValidator : AbstractValidator<CreateHomeworkDto>
{
    public CreateHomeworkDtoValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage(localizer["CourseIdRequired"]);

        RuleFor(x => x.Title)
            .NotNull().WithMessage(localizer["TitleRequired"])
            .NotEmpty().WithMessage(localizer["TitleRequired"])
            .MaximumLength(200).WithMessage(localizer["TitleMaxLength"]);

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage(localizer["DueDateFuture"]);

        RuleFor(x => x.MaxScore)
            .GreaterThan(0).WithMessage(localizer["MaxScorePositive"]);
    }
}