namespace StudentCoursePlatform.Application.DTOs.Courses.Responses;

public class CourseSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public int LessonCount { get; set; }
}
