using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Abstractions.Repositories;

public interface IEnrolmentRepository : IGenericRepository<Enrollment>
{
    Task<List<Enrollment>> GetStudentEnrollmentsAsync(Guid studentId, CancellationToken cancellationToken);
    Task<List<Enrollment>> GetCourseEnrollmentsAsync(Guid courseId, int page, int pageSize,
        CancellationToken cancellationToken);
}
