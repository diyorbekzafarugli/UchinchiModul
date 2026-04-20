namespace StudentCoursePlatform.Application.DTOs.Lessons.Responses;

public class GetLessonDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }

    public int Order { get; set; }

    public string? VideoUrl { get; set; }
    public string? FileUrl { get; set; }

    public bool IsPublished { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
