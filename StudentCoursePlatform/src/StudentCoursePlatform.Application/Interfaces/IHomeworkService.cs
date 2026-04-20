using StudentCoursePlatform.Application.Common;
using StudentCoursePlatform.Application.DTOs.Homeworks.Requests;
using StudentCoursePlatform.Application.DTOs.Homeworks.Responses;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Interfaces;

public interface IHomeworkService
{
    Task<Result<GetHomeworkDto>> CreateAsync(CreateHomeworkDto dto,
        CancellationToken cancellationToken);

    Task<Result<List<GetHomeworkDto>>> GetByCourseIdAsync(Guid courseId,
        PaginationParams pagination, CancellationToken cancellationToken);
}