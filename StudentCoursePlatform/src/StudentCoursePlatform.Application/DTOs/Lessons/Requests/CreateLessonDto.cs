namespace StudentCoursePlatform.Application.DTOs.Lessons.Requests;

public class CreateLessonDto
{
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public int Order { get; set; }
    public string? VideoUrl { get; set; }
    public string? FileUrl { get; set; }
    public bool IsPublished { get; set; } = false;
}
