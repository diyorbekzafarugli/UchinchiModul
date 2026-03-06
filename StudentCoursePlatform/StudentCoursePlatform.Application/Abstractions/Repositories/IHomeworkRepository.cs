using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Abstractions.Repositories;

public interface IHomeworkRepository : IGenericRepository<Homework>
{
    Task<Homework?> GetHomeworkByLessonIdAsync(Guid lessonId);
    Task<List<Homework>> GetHomeworksByCourseIdAsync(Guid courseId, int page, int pageSize);
}