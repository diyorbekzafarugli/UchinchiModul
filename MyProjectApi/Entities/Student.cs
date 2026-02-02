using MyProject.Interfaces;

namespace MyProject.Entities;

public class Student : IEntity
{
    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public int Age { get; set; }
    public DateTime CreatedAt { get; set; }
}
