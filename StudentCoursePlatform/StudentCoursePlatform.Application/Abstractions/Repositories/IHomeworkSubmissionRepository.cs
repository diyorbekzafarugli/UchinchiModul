using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Abstractions.Repositories;

public interface IHomeworkSubmissionRepository : IGenericRepository<HomeworkSubmission>
{
    Task<HomeworkSubmission?> GetByHomeworkIdAsync(Guid homeworkId);

    Task<HomeworkSubmission?> GetByStudentIdAsync(Guid studentId);

}
