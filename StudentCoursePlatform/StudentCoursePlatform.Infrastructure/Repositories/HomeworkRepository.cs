using Microsoft.EntityFrameworkCore;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Domain.Entities;
using StudentCoursePlatform.Infrastructure.Persistence;

namespace StudentCoursePlatform.Infrastructure.Repositories;

public class HomeworkRepository(AppDbContext appDbContext) : GenericRepository<Homework>(appDbContext), IHomeworkRepository
{
    public async Task<Homework?> GetHomeworkByLessonIdAsync(Guid lessonId)
    {
        return await _dbContext.Homeworks
            .FirstOrDefaultAsync(h => h.LessonId == lessonId);
    }

    public async Task<List<Homework>> GetHomeworksByCourseIdAsync(Guid courseId, int page, int pageSize)
    {
        return await _dbContext.Homeworks
            .Where(h => h.CourseId == courseId)
            .OrderByDescending(h => h.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
