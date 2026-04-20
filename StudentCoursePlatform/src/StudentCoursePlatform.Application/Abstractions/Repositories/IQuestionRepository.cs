using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Abstractions.Repositories;

public interface IQuestionRepository : IGenericRepository<Question>
{
    Task<List<Question>> GetQuestionsByQuizIdAsync(Guid quizId, int page, int pageSize,
        CancellationToken cancellationToken);
}
