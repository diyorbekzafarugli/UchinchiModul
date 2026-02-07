using System.ComponentModel.DataAnnotations;

namespace Product.Api.Dtos;

public class CreateProductDto
{
    [Required, MinLength(3)]
    public string Name { get; set; } = string.Empty;
    [Range(0.01, double.MaxValue, ErrorMessage = "narxi 0 dan katta bo'lishi kerak")]
    public decimal Price { get; set; }
    [Range(0.01, double.MaxValue, ErrorMessage = "0 dan katta bo'lishi kerak")]
    public int Stock { get; set; }
    [Required, MinLength(1)]
    public string Category { get; set; } = string.Empty;
}
