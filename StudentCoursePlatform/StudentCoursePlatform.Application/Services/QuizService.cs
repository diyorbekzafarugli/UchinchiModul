using FluentValidation;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Application.DTOs.Quizzes.Requests;
using StudentCoursePlatform.Application.DTOs.Quizzes.Responses;
using StudentCoursePlatform.Application.Interfaces;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Services;

public class QuizService : IQuizService
{
    private readonly IQuizRepository _quizRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IStringLocalizer<QuizService> _localizer;
    private readonly IValidator<CreateQuizDto> _validator;
    private readonly ILogger<QuizService> _logger;

    public QuizService(IQuizRepository quizRepository,
                       ICourseRepository courseRepository,
                       ICurrentUserService currentUser,
                       IStringLocalizer<QuizService> localizer,
                       IValidator<CreateQuizDto> validator,
                       ILogger<QuizService> logger)
    {
        _quizRepository = quizRepository;
        _courseRepository = courseRepository;
        _currentUser = currentUser;
        _localizer = localizer;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<GetQuizDto>> CreateAsync(CreateQuizDto dto,
        CancellationToken cancellationToken)
    {
        var teacherId = _currentUser.UserId;

        var validateResult = _validator.Validate(dto);
        if (!validateResult.IsValid)
        {
            var errors = string.Join("; ", validateResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning("Validation failed: {errors}", errors);
            return Result<GetQuizDto>.Fail(errors);
        }

        var course = await _courseRepository.GetByIdAsync(dto.CourseId, cancellationToken);
        if (course == null || course.TeacherId != teacherId)
        {
            return Result<GetQuizDto>.Fail(_localizer["CourseNotFoundOrForbidden"]);
        }

        var quiz = new Quiz
        {
            CourseId = dto.CourseId,
            Title = dto.Title,
            Description = dto.Description,
            TimeLimitMinutes = dto.TimeLimitMinutes,
            PassScore = dto.PassScore
        };

        await _quizRepository.AddAsync(quiz, cancellationToken);

        return Result<GetQuizDto>.Success(new GetQuizDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = quiz.Description,
            TimeLimitMinutes = quiz.TimeLimitMinutes,
            PassScore = quiz.PassScore
        });
    }

    public async Task<Result<GetQuizDto>> GetByCourseIdAsync(Guid courseId,
        CancellationToken cancellationToken)
    {
        var quiz = await _quizRepository.GetQuizByCourseId(courseId, cancellationToken);

        if (quiz == null)
        {
            return Result<GetQuizDto>.Fail(_localizer["QuizNotFound"]);
        }

        return Result<GetQuizDto>.Success(new GetQuizDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Description = quiz.Description,
            TimeLimitMinutes = quiz.TimeLimitMinutes,
            PassScore = quiz.PassScore
        });
    }
}