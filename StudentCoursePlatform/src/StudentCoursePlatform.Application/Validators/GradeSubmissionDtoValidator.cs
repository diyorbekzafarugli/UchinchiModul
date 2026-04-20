using FluentValidation;
using Microsoft.Extensions.Localization;
using StudentCoursePlatform.Application.DTOs.HomeworkSubmissions.Requests;
using StudentCoursePlatform.Application.Resources;

namespace StudentCoursePlatform.Application.Validators;

public class GradeSubmissionDtoValidator : AbstractValidator<GradeSubmissionDto>
{
    public GradeSubmissionDtoValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x.Score)
            .GreaterThanOrEqualTo(0).WithMessage(localizer["ScoreNonNegative"]);
    }
} 