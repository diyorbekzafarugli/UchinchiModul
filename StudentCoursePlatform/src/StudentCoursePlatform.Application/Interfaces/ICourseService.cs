using StudentCoursePlatform.Application.Common;
using StudentCoursePlatform.Application.DTOs.Courses.Requestes;
using StudentCoursePlatform.Application.DTOs.Courses.Requests;
using StudentCoursePlatform.Application.DTOs.Courses.Responses;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Interfaces;

public interface ICourseService
{
    Task<Result<List<CourseSummaryDto>>> GetAllAsync(string? searchTerm, PaginationParams pagination,
        CancellationToken cancellationToken);
    Task<Result<GetCourseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<List<GetCourseDto>>> GetByTeacherIdAsync(CancellationToken cancellationToken);
    Task<Result<GetCourseDto>> CreateAsync(CreateCourseDto dto, CancellationToken cancellationToken);
    Task<Result<GetCourseDto>> UpdateAsync(Guid id, UpdateCourseDto dto, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<bool>> PublishAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<bool>> UnpublishAsync(Guid id, CancellationToken cancellationToken);
}
