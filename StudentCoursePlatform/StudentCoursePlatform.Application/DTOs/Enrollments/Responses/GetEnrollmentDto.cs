namespace StudentCoursePlatform.Application.DTOs.Enrollments.Responses;

public class GetEnrollmentDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
}