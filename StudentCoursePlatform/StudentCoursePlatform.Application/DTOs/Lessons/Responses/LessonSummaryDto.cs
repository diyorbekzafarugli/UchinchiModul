namespace StudentCoursePlatform.Application.DTOs.Lessons.Responses;

public class LessonSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsPublished { get; set; }
}
