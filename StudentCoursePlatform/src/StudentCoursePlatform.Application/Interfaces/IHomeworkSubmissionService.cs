using StudentCoursePlatform.Application.DTOs.HomeworkSubmissions.Requests;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Interfaces;

public interface IHomeworkSubmissionService
{
    Task<Result<bool>> SubmitHomeworkAsync(SubmitHomeworkDto dto,
        CancellationToken cancellationToken);

    Task<Result<bool>> GradeSubmissionAsync(Guid submissionId,
        GradeSubmissionDto dto, CancellationToken cancellationToken);
}