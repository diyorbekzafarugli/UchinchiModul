using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Abstractions.Repositories;

public interface IAnswerOptionRepository : IGenericRepository<AnswerOption>
{
    Task<List<AnswerOption>> GetAnswersByQuestionIdAsync(Guid questionId);
}
