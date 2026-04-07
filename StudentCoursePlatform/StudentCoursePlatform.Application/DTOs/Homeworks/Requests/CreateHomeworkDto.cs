namespace StudentCoursePlatform.Application.DTOs.Homeworks.Requests;

public class CreateHomeworkDto
{
    public Guid CourseId { get; set; }
    public Guid? LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int MaxScore { get; set; }
}