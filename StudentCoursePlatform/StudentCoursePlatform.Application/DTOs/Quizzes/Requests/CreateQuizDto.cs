namespace StudentCoursePlatform.Application.DTOs.Quizzes.Requests;

public class CreateQuizDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TimeLimitMinutes { get; set; }
    public int PassScore { get; set; }
}