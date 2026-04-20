namespace StudentCoursePlatform.Domain.Entities;

public class Homework : AuditableEntity
{

    public Guid CourseId { get; set; }
    public Guid? LessonId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }
    public int MaxScore { get; set; }

    public Course Course { get; set; } = null!;
    public Lesson? Lesson { get; set; }

    public ICollection<HomeworkSubmission> Submissions { get; set; } = [];
}