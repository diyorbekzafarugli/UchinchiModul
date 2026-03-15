namespace StudentCoursePlatform.Domain.Entities;

public abstract class AuditableEntity : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = null;
}
