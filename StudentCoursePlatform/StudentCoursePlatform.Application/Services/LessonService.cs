using FluentValidation;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Application.DTOs.Lessons.Requests;
using StudentCoursePlatform.Application.DTOs.Lessons.Responses;
using StudentCoursePlatform.Application.Interfaces;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Services;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IStringLocalizer<LessonService> _localizer;
    private readonly IValidator<CreateLessonDto> _validator;
    private readonly ILogger<LessonService> _logger;
    private readonly ICourseRepository _courseRepository;

    public LessonService(ILessonRepository lessonRepository,
                         ICurrentUserService currentUser,
                         IStringLocalizer<LessonService> localizer,
                         IValidator<CreateLessonDto> validator,
                         ILogger<LessonService> logger,
                         ICourseRepository courseRepository)
    {
        _lessonRepository = lessonRepository;
        _currentUser = currentUser;
        _localizer = localizer;
        _validator = validator;
        _logger = logger;
        _courseRepository = courseRepository;
    }
    public async Task<Result<GetLessonDto>> CreateAsync(Guid courseId, CreateLessonDto dto,
        CancellationToken cancellationToken)
    {
        var teacherId = _currentUser.UserId;
        _logger.LogInformation("Teacher {TeacherId} is attempting to create lesson", teacherId);

        if (courseId == Guid.Empty)
            return Result<GetLessonDto>.Fail(_localizer["GuidIsEmpty"]);

        var validateResult = _validator.Validate(dto);
        if (!validateResult.IsValid)
        {
            var errors = string.Join("; ", validateResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning("Fatal {errors}", errors);
            return Result<GetLessonDto>.Fail(errors);
        }

        var course = await _courseRepository.GetByIdAsync(courseId, cancellationToken);
        if (course == null)
        {
            _logger.LogWarning("Course {CourseId} not found", courseId);
            return Result<GetLessonDto>.Fail(_localizer["CourseNotFound"]);
        }

        if (course.TeacherId != teacherId)
        {
            _logger.LogInformation("Teacher {TeacherId} tried to create someone else's course {CourseId}",
                teacherId, courseId);
            return Result<GetLessonDto>.Fail(_localizer["Forbidden"]);
        }

        var lesson = new Lesson
        {
            CourseId = courseId,
            Title = dto.Title,
            Content = dto.Content,
            Order = dto.Order,
            VideoUrl = dto.VideoUrl,
            FileUrl = dto.FileUrl,
            IsPublished = dto.IsPublished
        };

        await _lessonRepository.AddAsync(lesson, cancellationToken);
        _logger.LogInformation("Teacher's {TeacherId} lesson successfully added {LessonId}",
            teacherId, lesson.Id);
        return Result<GetLessonDto>.Success(MapToGetDto(lesson));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var teacherId = _currentUser.UserId;
        _logger.LogInformation("Teacher {TeacherId} is attempting to delete lesson {LessonId}",
            teacherId, id);

        var (lesson, error) = await GetAndValidateOwnershipAsync(id, teacherId, cancellationToken);

        if (error != null) return Result<bool>.Fail(error);

        await _lessonRepository.DeleteAsync(lesson!, cancellationToken);
        _logger.LogInformation("Lesson {LessonId} successfully deleted", id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<List<LessonSummaryDto>>> GetByCourseIdAsync(Guid courseId,
        CancellationToken cancellationToken)
    {
        var lessons = await _lessonRepository.GetByCourseIdAsync(courseId, cancellationToken);
        return Result<List<LessonSummaryDto>>.Success(lessons.Select(l => new LessonSummaryDto
        {
            Id = l.Id,
            Title = l.Title,
            Order = l.Order,
            IsPublished = l.IsPublished
        }).ToList());
    }

    public async Task<Result<GetLessonDto>> GetByIdAsync(Guid id,
        CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id, cancellationToken);
        if (lesson == null)
        {
            _logger.LogWarning("Lesson {LessonId} not found", id);
            return Result<GetLessonDto>.Fail(_localizer["LessonNotFound"]);
        }

        return Result<GetLessonDto>.Success(MapToGetDto(lesson));
    }

    public Task<Result<bool>> PublishAsync(Guid id, CancellationToken ct)
        => SetPublishStatusAsync(id, true, ct);

    public Task<Result<bool>> UnpublishAsync(Guid id, CancellationToken ct)
        => SetPublishStatusAsync(id, false, ct);

    public async Task<Result<GetLessonDto>> UpdateAsync(Guid id, UpdateLessonDto dto,
        CancellationToken cancellationToken)
    {
        var teacherId = _currentUser.UserId;
        _logger.LogInformation("Teacher {TeacherId} is attempting to update his lesson {LessonId}",
            teacherId, id);

        var (lesson, error) = await GetAndValidateOwnershipAsync(id, teacherId, cancellationToken);

        if (error != null)
            return Result<GetLessonDto>.Fail(error);

        lesson!.Title = dto.Title ?? lesson.Title;
        lesson.Content = dto.Content ?? lesson.Content;
        lesson.Order = dto.Order ?? lesson.Order;
        lesson.VideoUrl = dto.VideoUrl ?? lesson.VideoUrl;
        lesson.FileUrl = dto.FileUrl ?? lesson.FileUrl;
        lesson.IsPublished = dto.IsPublished ?? lesson.IsPublished;

        await _lessonRepository.UpdateAsync(lesson, cancellationToken);
        _logger.LogInformation("Teacher's {TeacherId} lesson {LessonId} successfully updated", teacherId, id);
        return Result<GetLessonDto>.Success(MapToGetDto(lesson));
    }
    private async Task<Result<bool>> SetPublishStatusAsync(Guid id, bool isPublished,
        CancellationToken cancellationToken)
    {
        var teacherId = _currentUser.UserId;
        var action = isPublished ? "publish" : "unpublish";

        _logger.LogInformation("Teacher {TeacherId} is attempting to {Action} lesson {LessonId}",
            teacherId, action, id);

        var (lesson, error) = await GetAndValidateOwnershipAsync(id, teacherId, cancellationToken);
        if (error != null) return Result<bool>.Fail(error);

        lesson!.IsPublished = isPublished;
        await _lessonRepository.UpdateAsync(lesson, cancellationToken);

        _logger.LogInformation("Teacher {TeacherId} successfully {Action}ed lesson {LessonId}",
            teacherId, action, id);

        return Result<bool>.Success(true);
    }
    private async Task<(Lesson? lesson, string? error)> GetAndValidateOwnershipAsync(
        Guid id, Guid teacherId, CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdWithCourseAsync(id, cancellationToken);
        if (lesson == null)
            return (null, _localizer["LessonNotFound"]);

        if (lesson.Course.TeacherId != teacherId)
            return (null, _localizer["Forbidden"]);

        return (lesson, null);
    }
    private GetLessonDto MapToGetDto(Lesson lesson)
    {
        return new GetLessonDto
        {
            Id = lesson.Id,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Content = lesson.Content,
            Order = lesson.Order,
            VideoUrl = lesson.VideoUrl,
            FileUrl = lesson.FileUrl,
            IsPublished = lesson.IsPublished,
            CreatedAt = lesson.CreatedAt,
            UpdatedAt = lesson.UpdatedAt
        };
    }
}
