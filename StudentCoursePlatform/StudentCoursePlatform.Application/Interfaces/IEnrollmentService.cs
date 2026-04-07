using StudentCoursePlatform.Application.DTOs.Enrollments.Requests;
using StudentCoursePlatform.Application.DTOs.Enrollments.Responses;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Interfaces;

public interface IEnrollmentService
{
    Task<Result<bool>> EnrollCourseAsync(EnrollCourseDto dto, 
        CancellationToken cancellationToken);
    Task<Result<List<GetEnrollmentDto>>> GetMyEnrollmentsAsync(CancellationToken cancellationToken);
}