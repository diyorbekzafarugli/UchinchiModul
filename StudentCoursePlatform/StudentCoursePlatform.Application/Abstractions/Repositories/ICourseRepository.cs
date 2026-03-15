using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Abstractions.Repositories;

public interface ICourseRepository : IGenericRepository<Course>
{
    Task<List<Course>> GetCoursesAsync(string? searchTerm, int page, int pageSize,
        CancellationToken cancellationToken);
    Task<List<Course>> GetCourseByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken);
    Task<Course?> GetCourseByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken);
}
