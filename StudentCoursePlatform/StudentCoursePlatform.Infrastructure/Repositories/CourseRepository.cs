using Microsoft.EntityFrameworkCore;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Domain.Entities;
using StudentCoursePlatform.Infrastructure.Persistence;

namespace StudentCoursePlatform.Infrastructure.Repositories;

public class CourseRepository : GenericRepository<Course>, ICourseRepository
{
    public CourseRepository(AppDbContext dbContext) : base(dbContext)
    {

    }
    public async Task<Course?> GetCourseByTeacherId(Guid id)
    {
        return await _dbContext.Courses.FirstOrDefaultAsync(c => c.TeacherId == id);
    }

    public async Task<List<Course>> GetCoursesAsync(string searchTerm, int page, int pageSize)
    {
        return await _dbContext.Courses
            .Where(c => c.Title.Contains(searchTerm) || c.Description.Contains(searchTerm))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
