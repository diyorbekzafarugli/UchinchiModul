using Microsoft.EntityFrameworkCore;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Domain.Entities;
using StudentCoursePlatform.Infrastructure.Persistence;

namespace StudentCoursePlatform.Infrastructure.Repositories;

public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
{
    public QuestionRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
    public async Task<List<Question>> GetQuestionsByQuizIdAsync(Guid quizId, int page, int pageSize,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Questions
            .Where(q => q.QuizId == quizId)
            .OrderBy(q => q.Order)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
