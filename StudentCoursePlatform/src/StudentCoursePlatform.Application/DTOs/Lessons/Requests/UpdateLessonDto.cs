namespace StudentCoursePlatform.Application.DTOs.Lessons.Requests;

public class UpdateLessonDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int? Order { get; set; }
    public string? VideoUrl { get; set; }
    public string? FileUrl { get; set; }
    public bool? IsPublished { get; set; }
}
