using Microsoft.EntityFrameworkCore;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Domain.Entities;
using StudentCoursePlatform.Infrastructure.Persistence;

namespace StudentCoursePlatform.Infrastructure.Repositories;

public class CourseRepository : GenericRepository<Course>, ICourseRepository
{
    public CourseRepository(AppDbContext dbContext) : base(dbContext) { }

    public async Task<Course?> GetCourseByIdWithDetailsAsync(Guid id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Courses
            .Include(c => c.Teacher)
            .Include(c => c.Lessons)
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<List<Course>> GetCourseByTeacherIdAsync(Guid teacherId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Courses
            .Include(c => c.Teacher)
            .Include(c => c.Lessons)
            .Include(c => c.Enrollments)
            .Where(c => c.TeacherId == teacherId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Course>> GetCoursesAsync(string? searchTerm, int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Courses
            .Include(c => c.Teacher)
            .Include(c => c.Lessons)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(c =>
                c.Title.Contains(searchTerm.Trim()) ||
                c.Description.Contains(searchTerm.Trim()));

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
