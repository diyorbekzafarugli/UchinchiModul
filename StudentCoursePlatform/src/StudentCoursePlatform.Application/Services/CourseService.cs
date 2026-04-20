using FluentValidation;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Application.Common;
using StudentCoursePlatform.Application.DTOs.Courses.Requestes;
using StudentCoursePlatform.Application.DTOs.Courses.Requests;
using StudentCoursePlatform.Application.DTOs.Courses.Responses;
using StudentCoursePlatform.Application.Interfaces;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IStringLocalizer<CourseService> _localizer;
    private readonly IValidator<CreateCourseDto> _validator;
    private readonly ILogger<CourseService> _logger;
    private readonly ICurrentUserService _currentUser;
    private readonly IUserRepository _userRepository;

    public CourseService(ICourseRepository courseRepository,
                         IStringLocalizer<CourseService> localizer,
                         IValidator<CreateCourseDto> validator,
                         ILogger<CourseService> logger,
                         ICurrentUserService currentUser,
                         IUserRepository userRepository)
    {
        _courseRepository = courseRepository;
        _localizer = localizer;
        _validator = validator;
        _logger = logger;
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    public async Task<Result<GetCourseDto>> CreateAsync(CreateCourseDto dto,
        CancellationToken cancellationToken)
    {
        var teacherId = _currentUser.UserId;
        _logger.LogInformation("Teacher {TeacherId} is attempting to create course", teacherId);

        var validationResult = _validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for Teacher {TeacherId}", teacherId);
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result<GetCourseDto>.Fail(errors);
        }

        var teacherFromDb = await _userRepository.GetByIdAsync(teacherId, cancellationToken);
        if (teacherFromDb == null)
        {
            _logger.LogWarning("User {TeacherId} not found or is not a teacher", teacherId);
            return Result<GetCourseDto>.Fail(_localizer["TeacherNotFound"]);
        }

        var course = new Course
        {
            Title = dto.Title,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            TeacherId = dto.TeacherId
        };

        await _courseRepository.AddAsync(course, cancellationToken);
        _logger.LogInformation("Course successfully created by Teacher {TeacherId}", teacherId);

        return Result<GetCourseDto>.Success(new GetCourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            ImageUrl = course.ImageUrl,
            TeacherId = course.TeacherId,
            TeacherName = teacherFromDb.FullName,
            IsPublished = course.IsPublished,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt,
            LessonCount = 0,
            EnrollmentCount = 0
        });
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var teacherId = _currentUser.UserId;

        _logger.LogInformation("Teacher {TeacherId} is attempting to delete course {CourseId}", teacherId, id);

        var course = await _courseRepository.GetByIdAsync(id, cancellationToken);
        if (course is null)
        {
            _logger.LogWarning("Course {CourseId} not found", id);
            return Result<bool>.Fail(_localizer["CourseNotFound"]);
        }

        if (course.TeacherId != teacherId)
        {
            _logger.LogWarning("Teacher {TeacherId} tried to delete someone else's course {CourseId}", teacherId, id);
            return Result<bool>.Fail(_localizer["Forbidden"]);
        }

        await _courseRepository.DeleteAsync(course, cancellationToken);
        _logger.LogInformation("Course {CourseId} successfully deleted by Teacher {TeacherId}", id, teacherId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<List<CourseSummaryDto>>> GetAllAsync(string? searchTerm, PaginationParams pagination,
        CancellationToken cancellationToken)
    {

        _logger.LogInformation("Getting courses, page {Page}, pageSize {PageSize}", pagination.Page, pagination.PageSize);

        var courses = await _courseRepository.GetCoursesAsync(searchTerm, pagination.Page, pagination.PageSize,
            cancellationToken);

        var result = courses.Select(c => new CourseSummaryDto
        {
            Id = c.Id,
            Title = c.Title,
            ImageUrl = c.ImageUrl,
            TeacherName = c.Teacher.FullName,
            IsPublished = c.IsPublished,
            LessonCount = c.Lessons.Count
        }).ToList();

        return Result<List<CourseSummaryDto>>.Success(result);
    }

    public async Task<Result<GetCourseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Course {CourseId} is getting", id);
        var course = await _courseRepository.GetCourseByIdWithDetailsAsync(id, cancellationToken);

        if (course == null)
        {
            _logger.LogWarning("Corse {CourseId} is incorrect, course not found", id);
            return Result<GetCourseDto>.Fail(_localizer["CourseNotFound"]);
        }

        return Result<GetCourseDto>.Success(MapToGetDto(course));
    }

    public async Task<Result<List<GetCourseDto>>> GetByTeacherIdAsync(CancellationToken cancellationToken)
    {
        var teacherId = _currentUser.UserId;
        _logger.LogInformation("Course {TeacherId} is getting", teacherId);
        var courses = await _courseRepository.GetCourseByTeacherIdAsync(teacherId, cancellationToken);

        return Result<List<GetCourseDto>>.Success(courses.Select(course => MapToGetDto(course)).ToList());
    }

    public Task<Result<bool>> PublishAsync(Guid id, CancellationToken ct) =>
        SetPublishStatusAsync(id, true, ct);
    public Task<Result<bool>> UnpublishAsync(Guid id, CancellationToken ct) =>
        SetPublishStatusAsync(id, false, ct);

    public async Task<Result<GetCourseDto>> UpdateAsync(Guid id, UpdateCourseDto dto, CancellationToken cancellationToken)
    {
        var teacherId = _currentUser.UserId;
        _logger.LogInformation("Teacher {TeacherId} is attempting to update their course {CourseId}", teacherId, id);

        var course = await _courseRepository.GetCourseByIdWithDetailsAsync(id, cancellationToken);
        if (course == null)
        {
            _logger.LogWarning("Course {CourseId} not found", id);
            return Result<GetCourseDto>.Fail(_localizer["CourseNotFound"]);
        }

        if (course.TeacherId != teacherId)
        {
            _logger.LogWarning("Teacher {TeacherId} tried to update someone else's course {CourseId}", teacherId, id);
            return Result<GetCourseDto>.Fail(_localizer["Forbidden"]);
        }

        course.Title = dto.Title ?? course.Title;
        course.Description = dto.Description ?? course.Description;
        course.ImageUrl = dto.ImageUrl ?? course.ImageUrl;
        await _courseRepository.UpdateAsync(course, cancellationToken);

        _logger.LogInformation("Course {CourseId} updated successfuly by teacher {TeacherId}", id, teacherId);

        return Result<GetCourseDto>.Success(MapToGetDto(course));
    }

    private async Task<(Course? course, string? error)> GetAndValidateOwnershipAsync(
         Guid id, Guid teacherId, CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(id, cancellationToken);
        if (course == null)
            return (null, _localizer["CourseNotFound"]);

        if (course.TeacherId != teacherId)
            return (null, _localizer["Forbidden"]);

        return (course, null);
    }

    private async Task<Result<bool>> SetPublishStatusAsync(Guid id, bool isPublished, CancellationToken ct)
    {
        var teacherId = _currentUser.UserId;
        var action = isPublished ? "publish" : "unpublish";
        _logger.LogInformation("Teacher {TeacherId} is attempting to {Action} course {CourseId}", teacherId, action, id);

        var (course, error) = await GetAndValidateOwnershipAsync(id, teacherId, ct);
        if (error != null) return Result<bool>.Fail(error);

        course!.IsPublished = isPublished;
        await _courseRepository.UpdateAsync(course, ct);
        _logger.LogInformation("Teacher {TeacherId} successfully {Action}ed course {CourseId}", teacherId, action, id);
        return Result<bool>.Success(true);
    }

    private GetCourseDto MapToGetDto(Course course) => new GetCourseDto
    {
        Id = course.Id,
        Title = course.Title,
        Description = course.Description,
        ImageUrl = course.ImageUrl,
        TeacherId = course.TeacherId,
        TeacherName = course.Teacher.FullName,
        IsPublished = course.IsPublished,
        CreatedAt = course.CreatedAt,
        UpdatedAt = course.UpdatedAt,
        LessonCount = course.Lessons.Count,
        EnrollmentCount = course.Enrollments.Count
    };
}
