using Order.Api.Entities;

namespace Order.Api.Dtos;

public class UpdateOrderStatusDto
{
    public OrderStatus Status { get; set; }
}
