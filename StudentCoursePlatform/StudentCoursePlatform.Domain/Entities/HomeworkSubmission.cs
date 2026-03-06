namespace StudentCoursePlatform.Domain.Entities;

public class HomeworkSubmission : IEntity
{
    public Guid Id { get; set; }

    public Guid HomeworkId { get; set; }
    public Guid StudentId { get; set; }

    public string? Content { get; set; }
    public string? FileUrl { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public int? Score { get; set; }
    public string? Feedback { get; set; }

    public Homework Homework { get; set; } = null!;
    public User Student { get; set; } = null!;
}