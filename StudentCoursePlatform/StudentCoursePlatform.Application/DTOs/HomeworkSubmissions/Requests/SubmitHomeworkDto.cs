namespace StudentCoursePlatform.Application.DTOs.HomeworkSubmissions.Requests;

public class SubmitHomeworkDto
{
    public Guid HomeworkId { get; set; }
    public string? Content { get; set; }
    public string? FileUrl { get; set; }
}