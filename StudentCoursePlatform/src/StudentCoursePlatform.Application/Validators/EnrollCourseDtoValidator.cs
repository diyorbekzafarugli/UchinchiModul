using FluentValidation;
using Microsoft.Extensions.Localization;
using StudentCoursePlatform.Application.DTOs.Enrollments.Requests;
using StudentCoursePlatform.Application.Resources;

namespace StudentCoursePlatform.Application.Validators;

public class EnrollCourseDtoValidator : AbstractValidator<EnrollCourseDto>
{
    public EnrollCourseDtoValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage(localizer["CourseIdRequired"]);
    }
}