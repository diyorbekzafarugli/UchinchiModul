namespace StudentCoursePlatform.Application.DTOs.Courses.Requestes;

public class CreateCourseDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public Guid TeacherId { get; set; }
}
