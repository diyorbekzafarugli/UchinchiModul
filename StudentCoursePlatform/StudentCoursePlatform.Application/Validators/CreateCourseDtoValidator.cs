using FluentValidation;
using Microsoft.Extensions.Localization;
using StudentCoursePlatform.Application.DTOs.Courses.Requestes;
using StudentCoursePlatform.Application.Resources;

namespace StudentCoursePlatform.Application.Validators;

public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
{
    public CreateCourseDtoValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithMessage(localizer["TitleRequired"])
            .MinimumLength(3).WithMessage(localizer["TitleMinLength"])
            .MaximumLength(100).WithMessage(localizer["TitleMaxLength"]);

        RuleFor(c => c.Description)
            .NotEmpty().WithMessage(localizer["DescriptionRequired"])
            .MinimumLength(10).WithMessage(localizer["DescriptionMinLength"])
            .MaximumLength(1000).WithMessage(localizer["DescriptionMaxLength"]);

        RuleFor(c => c.ImageUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .When(c => c.ImageUrl != null)
            .WithMessage(localizer["InvalidImageUrl"]);
    }
}
