using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Abstractions.Repositories;

public interface IQuizRepository : IGenericRepository<Quiz>
{
    Task<Quiz?> GetQuizByCourseId(Guid courseId);
}
