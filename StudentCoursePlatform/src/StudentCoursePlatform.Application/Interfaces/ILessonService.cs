using StudentCoursePlatform.Application.DTOs.Lessons.Requests;
using StudentCoursePlatform.Application.DTOs.Lessons.Responses;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Interfaces;

public interface ILessonService
{
    Task<Result<GetLessonDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<List<LessonSummaryDto>>> GetByCourseIdAsync(Guid courseId,
        CancellationToken cancellationToken);
    Task<Result<GetLessonDto>> CreateAsync(Guid courseId, CreateLessonDto dto,
        CancellationToken cancellationToken);
    Task<Result<GetLessonDto>> UpdateAsync(Guid id, UpdateLessonDto dto,
        CancellationToken cancellationToken);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<bool>> PublishAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<bool>> UnpublishAsync(Guid id, CancellationToken cancellationToken);
}