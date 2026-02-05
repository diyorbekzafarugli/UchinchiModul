using System.Reflection.Metadata.Ecma335;

namespace Order.Api.Dtos;

public class UpdateOrderDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}
