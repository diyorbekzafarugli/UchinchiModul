namespace StudentCoursePlatform.Application.DTOs.Quizzes.Responses;

public class GetQuizDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TimeLimitMinutes { get; set; }
    public int PassScore { get; set; }
}