using FluentValidation;
using Microsoft.Extensions.Localization;
using StudentCoursePlatform.Application.DTOs.Lessons.Requests;
using StudentCoursePlatform.Application.Resources;

namespace StudentCoursePlatform.Application.Validators;

public class CreateLessonDtoValidator : AbstractValidator<CreateLessonDto>
{
    public CreateLessonDtoValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(l => l.Title)
            .NotEmpty().WithMessage(localizer["TitleRequired"])
            .MinimumLength(3).WithMessage(localizer["TitleMinLength"])
            .MaximumLength(100).WithMessage(localizer["TitleMaxLength"]);

        RuleFor(l => l.Order)
            .GreaterThanOrEqualTo(0).WithMessage(localizer["OrderNegative"]);

        RuleFor(l => l.VideoUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .When(l => l.VideoUrl != null)
            .WithMessage(localizer["InvalidVideoUrl"]);

        RuleFor(l => l.FileUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .When(l => l.FileUrl != null)
            .WithMessage(localizer["InvalidFileUrl"]);
    }
}
