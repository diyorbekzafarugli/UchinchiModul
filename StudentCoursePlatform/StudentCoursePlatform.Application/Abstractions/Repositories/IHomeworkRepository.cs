using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Abstractions.Repositories;

public interface IHomeworkRepository : IGenericRepository<Homework>
{
    Task<Homework?> GetHomeworkByLessonIdAsync(Guid lessonId, CancellationToken cancellationToken);
    Task<List<Homework>> GetHomeworksByCourseIdAsync(Guid courseId, int page, int pageSize,
        CancellationToken cancellationToken);
}