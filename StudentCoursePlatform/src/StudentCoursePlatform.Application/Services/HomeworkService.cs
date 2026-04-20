using FluentValidation;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Application.Common;
using StudentCoursePlatform.Application.DTOs.Homeworks.Requests;
using StudentCoursePlatform.Application.DTOs.Homeworks.Responses;
using StudentCoursePlatform.Application.Interfaces;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Services;

public class HomeworkService : IHomeworkService
{
    private readonly IHomeworkRepository _homeworkRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IStringLocalizer<HomeworkService> _localizer;
    private readonly IValidator<CreateHomeworkDto> _validator;
    private readonly ILogger<HomeworkService> _logger;

    public HomeworkService(IHomeworkRepository homeworkRepository,
                           ICourseRepository courseRepository,
                           ICurrentUserService currentUser,
                           IStringLocalizer<HomeworkService> localizer,
                           IValidator<CreateHomeworkDto> validator,
                           ILogger<HomeworkService> logger)
    {
        _homeworkRepository = homeworkRepository;
        _courseRepository = courseRepository;
        _currentUser = currentUser;
        _localizer = localizer;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<GetHomeworkDto>> CreateAsync(CreateHomeworkDto dto,
        CancellationToken cancellationToken)
    {
        var teacherId = _currentUser.UserId;
        _logger.LogInformation("Teacher {TeacherId} is attempting to create homework", teacherId);

        var validateResult = _validator.Validate(dto);
        if (!validateResult.IsValid)
        {
            var errors = string.Join("; ", validateResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning("Validation failed: {errors}", errors);
            return Result<GetHomeworkDto>.Fail(errors);
        }

        var course = await _courseRepository.GetByIdAsync(dto.CourseId, cancellationToken);
        if (course == null)
            return Result<GetHomeworkDto>.Fail(_localizer["CourseNotFound"]);

        if (course.TeacherId != teacherId)
            return Result<GetHomeworkDto>.Fail(_localizer["Forbidden"]);

        var homework = new Homework
        {
            CourseId = dto.CourseId,
            LessonId = dto.LessonId,
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            MaxScore = dto.MaxScore
        };

        await _homeworkRepository.AddAsync(homework, cancellationToken);
        _logger.LogInformation("Homework {HomeworkId} created successfully", homework.Id);

        return Result<GetHomeworkDto>.Success(new GetHomeworkDto
        {
            Id = homework.Id,
            Title = homework.Title,
            Description = homework.Description,
            DueDate = homework.DueDate,
            MaxScore = homework.MaxScore
        });
    }

    public async Task<Result<List<GetHomeworkDto>>> GetByCourseIdAsync(Guid courseId,
        PaginationParams pagination, CancellationToken cancellationToken)
    {

        var course = _courseRepository.GetByIdAsync(courseId, cancellationToken);
        if (course is null) return Result<List<GetHomeworkDto>>.Fail("Course not found");

        var homeworks = await _homeworkRepository.GetHomeworksByCourseIdAsync(courseId,
            pagination.Page, pagination.PageSize, cancellationToken);

        var result = homeworks.Select(h => new GetHomeworkDto
        {
            Id = h.Id,
            Title = h.Title,
            Description = h.Description,
            DueDate = h.DueDate,
            MaxScore = h.MaxScore
        }).ToList();

        return Result<List<GetHomeworkDto>>.Success(result);
    }
}