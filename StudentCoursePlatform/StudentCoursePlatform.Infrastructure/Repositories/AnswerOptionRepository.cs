using Microsoft.EntityFrameworkCore;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Domain.Entities;
using StudentCoursePlatform.Infrastructure.Persistence;

namespace StudentCoursePlatform.Infrastructure.Repositories;

public class AnswerOptionRepository : GenericRepository<AnswerOption>, IAnswerOptionRepository
{
    public AnswerOptionRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task<List<AnswerOption>> GetAnswersByQuestionIdAsync(Guid questionId)
    {
        return await _dbContext.AnswerOptions
            .Where(a => a.QuestionId == questionId)
            .ToListAsync();
    }
}
