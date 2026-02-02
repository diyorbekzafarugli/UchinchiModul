namespace MyProject.Dtos;

public class ProductResultDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
}
