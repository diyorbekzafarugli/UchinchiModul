namespace StudentCoursePlatform.Domain.Entities;

public class Course : AuditableEntity
{

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }

    public Guid TeacherId { get; set; }

    public bool IsPublished { get; set; } = false;

    public User Teacher { get; set; } = null!;

    public ICollection<Lesson> Lessons   { get; set; } = new List<Lesson>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Homework> Homeworks { get; set; } = new List<Homework>();
    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}