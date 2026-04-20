using Microsoft.EntityFrameworkCore;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Domain.Entities;
using StudentCoursePlatform.Infrastructure.Persistence;

namespace StudentCoursePlatform.Infrastructure.Repositories;

public class EnrolmentRepository(AppDbContext appDbContext) :
    GenericRepository<Enrollment>(appDbContext), IEnrolmentRepository
{
    public Task<List<Enrollment>> GetCourseEnrollmentsAsync(Guid courseId, int page, int pageSize,
        CancellationToken cancellationToken)
    {
        return _dbContext.Enrollments
            .Where(e => e.CourseId == courseId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Enrollment>> GetStudentEnrollmentsAsync(Guid studentId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Enrollments
            .Where(e => e.StudentId == studentId)
            .ToListAsync(cancellationToken);
    }
}
