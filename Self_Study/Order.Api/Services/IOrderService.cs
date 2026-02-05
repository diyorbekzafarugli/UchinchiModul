using Order.Api.Dtos;
using Order.Api.Entities;

namespace Order.Api.Services;

public interface IOrderService
{
    public Guid Create(CreateOrderDto createOrderDto);
    public bool Update(UpdateOrderDto updateOrderDto);
    public bool Delete(Guid id);
    public OrderResultDto? GetById(Guid id);
    public List<OrderResultDto> GetAll();
    public bool UpdateStatus(Guid id, OrderStatus newStatus);
}
