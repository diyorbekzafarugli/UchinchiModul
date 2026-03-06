using Microsoft.EntityFrameworkCore;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Domain.Entities;
using StudentCoursePlatform.Infrastructure.Persistence;

namespace StudentCoursePlatform.Infrastructure.Repositories;

public class QuizRepository : GenericRepository<Quiz>, IQuizRepository
{
    public QuizRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task<Quiz?> GetQuizByCourseId(Guid courseId)
    {
        return await _dbContext.Quizzes
            .FirstOrDefaultAsync(q => q.CourseId == courseId);
    }
}
