using MyProject.Interfaces;

namespace MyProject.Entities;

public class Product : IEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}
