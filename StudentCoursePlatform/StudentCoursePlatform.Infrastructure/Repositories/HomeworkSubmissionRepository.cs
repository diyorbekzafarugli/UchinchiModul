using Microsoft.EntityFrameworkCore;
using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Domain.Entities;
using StudentCoursePlatform.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentCoursePlatform.Infrastructure.Repositories;

public class HomeworkSubmissionRepository : GenericRepository<HomeworkSubmission>,
    IHomeworkSubmissionRepository
{
    public HomeworkSubmissionRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }

    public async Task<HomeworkSubmission?> GetByHomeworkIdAsync(Guid homeworkId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.HomeworkSubmissions
            .FirstOrDefaultAsync(h => h.HomeworkId == homeworkId, cancellationToken);
    }

    public async Task<HomeworkSubmission?> GetByStudentIdAsync(Guid studentId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.HomeworkSubmissions
            .FirstOrDefaultAsync(h => h.StudentId == studentId, cancellationToken);
    }
}
