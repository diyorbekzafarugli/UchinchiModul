namespace StudentCoursePlatform.Domain.Entities;

public class Lesson : AuditableEntity
{
    public Guid CourseId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }

    public int Order { get; set; }

    public string? VideoUrl { get; set; }
    public string? FileUrl { get; set; }

    public bool IsPublished { get; set; } = false;

    public Course Course { get; set; } = null!;
    public ICollection<Homework> Homeworks { get; set; } = new List<Homework>();
}