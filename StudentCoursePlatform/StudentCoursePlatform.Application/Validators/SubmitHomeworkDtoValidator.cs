using FluentValidation;
using Microsoft.Extensions.Localization;
using StudentCoursePlatform.Application.DTOs.HomeworkSubmissions.Requests;
using StudentCoursePlatform.Application.Resources;

namespace StudentCoursePlatform.Application.Validators;

public class SubmitHomeworkDtoValidator : AbstractValidator<SubmitHomeworkDto>
{
    public SubmitHomeworkDtoValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.HomeworkId)
            .NotEmpty().WithMessage(localizer["HomeworkIdRequired"]);

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Content) || !string.IsNullOrWhiteSpace(x.FileUrl))
            .WithMessage(localizer["ContentOrFileRequired"]);
    }
}