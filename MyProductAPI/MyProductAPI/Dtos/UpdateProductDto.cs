namespace MyProductAPI.Dtos;

public class UpdateProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
}
