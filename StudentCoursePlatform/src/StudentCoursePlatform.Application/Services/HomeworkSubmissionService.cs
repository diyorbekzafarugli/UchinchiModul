using FluentValidation;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Application.DTOs.HomeworkSubmissions.Requests;
using StudentCoursePlatform.Application.Interfaces;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Services;

public class HomeworkSubmissionService : IHomeworkSubmissionService
{
    private readonly IHomeworkSubmissionRepository _submissionRepository;
    private readonly IHomeworkRepository _homeworkRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IStringLocalizer<HomeworkSubmissionService> _localizer;
    private readonly IValidator<SubmitHomeworkDto> _submitValidator;
    private readonly IValidator<GradeSubmissionDto> _gradeValidator;
    private readonly ILogger<HomeworkSubmissionService> _logger;

    public HomeworkSubmissionService(IHomeworkSubmissionRepository submissionRepository,
                                     IHomeworkRepository homeworkRepository,
                                     ICurrentUserService currentUser,
                                     IStringLocalizer<HomeworkSubmissionService> localizer,
                                     IValidator<SubmitHomeworkDto> submitValidator,
                                     IValidator<GradeSubmissionDto> gradeValidator,
                                     ILogger<HomeworkSubmissionService> logger)
    {
        _submissionRepository = submissionRepository;
        _homeworkRepository = homeworkRepository;
        _currentUser = currentUser;
        _localizer = localizer;
        _submitValidator = submitValidator;
        _gradeValidator = gradeValidator;
        _logger = logger;
    }

    public async Task<Result<bool>> SubmitHomeworkAsync(SubmitHomeworkDto dto,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUser.UserId;

        var validateResult = _submitValidator.Validate(dto);
        if (!validateResult.IsValid)
        {
            var errors = string.Join("; ", validateResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning("Validation failed for submission: {errors}", errors);
            return Result<bool>.Fail(errors);
        }

        var homework = await _homeworkRepository.GetByIdAsync(dto.HomeworkId, cancellationToken);
        if (homework == null) return Result<bool>.Fail(_localizer["HomeworkNotFound"]);

        var submission = new HomeworkSubmission
        {
            HomeworkId = dto.HomeworkId,
            StudentId = studentId,
            Content = dto.Content,
            FileUrl = dto.FileUrl,
            SubmittedAt = DateTime.UtcNow
        };

        await _submissionRepository.AddAsync(submission, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> GradeSubmissionAsync(Guid submissionId,
        GradeSubmissionDto dto, CancellationToken cancellationToken)
    {
        var teacherId = _currentUser.UserId;

        var validateResult = _gradeValidator.Validate(dto);
        if (!validateResult.IsValid)
        {
            var errors = string.Join("; ", validateResult.Errors.Select(e => e.ErrorMessage));
            return Result<bool>.Fail(errors);
        }

        var submission = await _submissionRepository
            .GetByIdAsync(submissionId, cancellationToken);

        if (submission == null) return Result<bool>.Fail(_localizer["SubmissionNotFound"]);

        submission.Score = dto.Score;
        submission.Feedback = dto.Feedback;

        await _submissionRepository.UpdateAsync(submission, cancellationToken);
        return Result<bool>.Success(true);
    }
}