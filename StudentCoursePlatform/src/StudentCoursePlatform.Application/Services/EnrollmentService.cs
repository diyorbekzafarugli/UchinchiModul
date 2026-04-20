using FluentValidation;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Application.DTOs.Enrollments.Requests;
using StudentCoursePlatform.Application.DTOs.Enrollments.Responses;
using StudentCoursePlatform.Application.Interfaces;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrolmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IStringLocalizer<EnrollmentService> _localizer;
    private readonly IValidator<EnrollCourseDto> _validator;
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(IEnrolmentRepository enrollmentRepository,
                             ICourseRepository courseRepository,
                             ICurrentUserService currentUser,
                             IStringLocalizer<EnrollmentService> localizer,
                             IValidator<EnrollCourseDto> validator,
                             ILogger<EnrollmentService> logger)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _currentUser = currentUser;
        _localizer = localizer;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<bool>> EnrollCourseAsync(EnrollCourseDto dto,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUser.UserId;
        _logger.LogInformation("Student {StudentId} is attempting to enroll in course {CourseId}", studentId, dto.CourseId);

        var validateResult = _validator.Validate(dto);
        if (!validateResult.IsValid)
        {
            var errors = string.Join("; ", validateResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning("Validation failed: {errors}", errors);
            return Result<bool>.Fail(errors);
        }

        var course = await _courseRepository.GetByIdAsync(dto.CourseId, cancellationToken);
        if (course == null)
        {
            _logger.LogWarning("Course {CourseId} not found", dto.CourseId);
            return Result<bool>.Fail(_localizer["CourseNotFound"]);
        }

        var existingEnrollments = await _enrollmentRepository
            .GetStudentEnrollmentsAsync(studentId, cancellationToken);

        if (existingEnrollments.Any(e => e.CourseId == dto.CourseId))
        {
            _logger.LogWarning("Student {StudentId} is already enrolled in course {CourseId}", studentId, dto.CourseId);
            return Result<bool>.Fail(_localizer["AlreadyEnrolled"]);
        }

        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            CourseId = dto.CourseId
        };

        await _enrollmentRepository.AddAsync(enrollment, cancellationToken);
        _logger.LogInformation("Student {StudentId} successfully enrolled in course {CourseId}", studentId, dto.CourseId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<List<GetEnrollmentDto>>> GetMyEnrollmentsAsync(CancellationToken cancellationToken)
    {
        var studentId = _currentUser.UserId;
        var enrollments = await _enrollmentRepository
            .GetStudentEnrollmentsAsync(studentId, cancellationToken);

        var result = enrollments.Select(e => new GetEnrollmentDto
        {
            Id = e.Id,
            CourseId = e.CourseId,
            CourseTitle = e.Course?.Title ?? string.Empty,
            TeacherName = e.Course?.Teacher?.FullName ?? string.Empty,
            EnrolledAt = e.EnrolledAt
        }).ToList();

        return Result<List<GetEnrollmentDto>>.Success(result);
    }
}