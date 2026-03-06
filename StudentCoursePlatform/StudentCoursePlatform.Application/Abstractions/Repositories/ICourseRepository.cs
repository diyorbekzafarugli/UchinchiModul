using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Abstractions.Repositories;

public interface ICourseRepository : IGenericRepository<Course>
{
    Task<List<Course>> GetCoursesAsync(string searchTerm, int page, int pageSize);
    Task<Course?> GetCourseByTeacherId(Guid id);
}
