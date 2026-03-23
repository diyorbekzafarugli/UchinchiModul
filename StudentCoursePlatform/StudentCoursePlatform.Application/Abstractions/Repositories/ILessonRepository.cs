using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Abstractions.Repositories;

public interface ILessonRepository : IGenericRepository<Lesson>
{
    Task<List<Lesson>> GetLessonsByCourseIdAsync(Guid id, int page, int pageSize,
        CancellationToken cancellationToken);
    Task<List<Lesson>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken);
    Task<List<Lesson>> GetLessonsByTitle(string searchTerm, int page, int pageSize,
        CancellationToken cancellationToken);
    Task<Lesson?> GetCourseByOrder(int order, CancellationToken cancellationToken);
    Task<Lesson?> GetByIdWithCourseAsync(Guid id, CancellationToken cancellationToken);
}
