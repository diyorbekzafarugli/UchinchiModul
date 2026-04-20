namespace StudentCoursePlatform.Domain.Entities;

public class Quiz : IEntity
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public string Title { get; set; } = string.Empty;   
    public string? Description { get; set; }

    public int TimeLimitMinutes { get; set; }
    public int PassScore { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Course Course { get; set; } = null!;
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}