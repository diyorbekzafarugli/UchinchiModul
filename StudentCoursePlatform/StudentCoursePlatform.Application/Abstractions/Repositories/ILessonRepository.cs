using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Abstractions.Repositories;

public interface ILessonRepository : IGenericRepository<Lesson>
{
    Task<List<Lesson>> GetLessonsByCourseIdAsync(Guid id, int page, int pageSize);
    Task<List<Lesson>> GetLessonsByTitle(string searchTerm, int page, int pageSize);
    Task<Lesson?> GetCourseByOrder(int order);
}
