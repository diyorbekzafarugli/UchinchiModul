using Order.Api.Dtos;
using Order.Api.Entities;
using Order.Api.Repositories;

namespace Order.Api.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order.Api.Entities.Order> _repository;

    public OrderService(IRepository<Order.Api.Entities.Order> repository)
    {
        _repository = repository;
    }

    public Guid Create(CreateOrderDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CustomerName) || dto.CustomerName.Length < 3) return Guid.Empty;
        if (string.IsNullOrWhiteSpace(dto.PhoneNumber) || dto.PhoneNumber.Length < 9) return Guid.Empty;
        if (dto.TotalAmount <= 0) return Guid.Empty;

        var order = new Order.Api.Entities.Order
        {
            Id = Guid.NewGuid(),
            CustomerName = dto.CustomerName,
            PhoneNumber = dto.PhoneNumber,
            TotalAmount = dto.TotalAmount,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null    
        };

        return _repository.Add(order);
    }

    public List<OrderResultDto> GetAll()
    {
        var orders = _repository.GetAll();

        var result = new List<OrderResultDto>();
        foreach (var order in orders)
            result.Add(ToDto(order));

        return result;
    }

    public OrderResultDto? GetById(Guid id)
    {
        if (id == Guid.Empty) return null;

        var order = _repository.GetById(id);
        if (order is null) return null;

        return ToDto(order);
    }

    public bool Update(UpdateOrderDto dto)
    {
        if (dto.Id == Guid.Empty) return false;
        if (string.IsNullOrWhiteSpace(dto.CustomerName) || dto.CustomerName.Length < 3) return false;
        if (string.IsNullOrWhiteSpace(dto.PhoneNumber) || dto.PhoneNumber.Length < 9) return false;
        if (dto.TotalAmount <= 0) return false;

        var order = _repository.GetById(dto.Id);
        if (order is null) return false;

        order.CustomerName = dto.CustomerName;
        order.PhoneNumber = dto.PhoneNumber;
        order.TotalAmount = dto.TotalAmount;
        order.UpdatedAt = DateTime.UtcNow;

        return _repository.Update(order);
    }

    public bool Delete(Guid id)
    {
        if (id == Guid.Empty) return false;
        return _repository.Delete(id);
    }

    public bool UpdateStatus(Guid id, OrderStatus newStatus)
    {
        if (id == Guid.Empty) return false;

        var order = _repository.GetById(id);
        if (order is null) return false;

        var oldStatus = order.Status;

        // 1) Cancelled bo'lsa - o'zgartirib bo'lmaydi
        if (oldStatus == OrderStatus.Cancelled) return false;

        // 2) Shipped bo'lishi uchun oldin Paid bo'lishi shart
        if (newStatus == OrderStatus.Shipped && oldStatus != OrderStatus.Paid) return false;

        // 3) Pending bo'lsa: faqat Paid yoki Cancelled bo'lishi mumkin
        if (oldStatus == OrderStatus.Pending &&
            newStatus != OrderStatus.Paid &&
            newStatus != OrderStatus.Cancelled)
            return false;

        // 4) Paid bo'lsa: faqat Shipped yoki Cancelled bo'lishi mumkin
        if (oldStatus == OrderStatus.Paid &&
            newStatus != OrderStatus.Shipped &&
            newStatus != OrderStatus.Cancelled)
            return false;

        // 5) Shipped bo'lsa - endi o'zgarmasin 
        if (oldStatus == OrderStatus.Shipped) return false;

        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        return _repository.Update(order);
    }

    private static OrderResultDto ToDto(Order.Api.Entities.Order order)
    {
        return new OrderResultDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            PhoneNumber = order.PhoneNumber,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }
}
