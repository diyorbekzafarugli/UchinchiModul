namespace StudentCoursePlatform.Application.DTOs.Homeworks.Responses;

public class GetHomeworkDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public int MaxScore { get; set; }
}