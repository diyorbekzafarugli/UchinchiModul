namespace StudentCoursePlatform.Domain.Entities;

public class Enrollment : IEntity
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    public User Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
}