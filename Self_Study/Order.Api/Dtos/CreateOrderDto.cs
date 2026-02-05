namespace Order.Api.Dtos;

public class CreateOrderDto
{
    public string CustomerName { get; set; }
    public string PhoneNumber { get; set; }
    public decimal TotalAmount { get; set; }
}
