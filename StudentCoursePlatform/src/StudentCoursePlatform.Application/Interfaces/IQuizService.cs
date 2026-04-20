using StudentCoursePlatform.Application.DTOs.Quizzes.Requests;
using StudentCoursePlatform.Application.DTOs.Quizzes.Responses;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Interfaces;

public interface IQuizService
{
    Task<Result<GetQuizDto>> CreateAsync(CreateQuizDto dto,
        CancellationToken cancellationToken);

    Task<Result<GetQuizDto>> GetByCourseIdAsync(Guid courseId,
        CancellationToken cancellationToken);
}