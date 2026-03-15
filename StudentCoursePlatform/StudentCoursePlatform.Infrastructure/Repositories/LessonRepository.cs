using Microsoft.EntityFrameworkCore;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Domain.Entities;
using StudentCoursePlatform.Infrastructure.Persistence;

namespace StudentCoursePlatform.Infrastructure.Repositories;

public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
{
    public LessonRepository(AppDbContext dbContext) : base(dbContext)
    {

    }
    public async Task<Lesson?> GetCourseByOrder(int order, CancellationToken cancellationToken)
    {
        return await _dbContext.Lessons
            .FirstOrDefaultAsync(l => l.Order == order, cancellationToken);
    }

    public async Task<List<Lesson>> GetLessonsByCourseIdAsync(Guid id, int page, int pageSize,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Lessons
            .Where(l => l.CourseId == id)
            .OrderBy(l => l.Order)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Lesson>> GetLessonsByTitle(string searchTerm, int page, int pageSize,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Lessons
            .Where(l => l.Title.Contains(searchTerm) ||
            (l.Content ?? string.Empty).Contains(searchTerm))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
