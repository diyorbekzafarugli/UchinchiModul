
namespace Order.Api.Repositories;

using Order.Api.Entities;
using System.Text.Json;

public class OrderRepository : IRepository<Order>
{
    private readonly string _filePath;
    public OrderRepository()
    { 
        if (!Directory.Exists("Data")) Directory.CreateDirectory("Data");
        _filePath = Path.Combine("Data", "orders.json");
    }

    private List<Order> ReadFromFile()
    {
        if (!File.Exists(_filePath)) return new List<Order>();
        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Order>>(json) ?? new List<Order>();
    }

    private void WriteToFile(List<Order> orders)
    {
        var json = JsonSerializer.Serialize(orders, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
    public Guid Add(Order order)
    {
        var orders = ReadFromFile();
        if(orders.Any(o => o.Id == order.Id))
        {
            throw new InvalidOperationException("foydalanuvchu bu Id bilan alaqachon ro'yxatdan o'tgan");
        }
        orders.Add(order);
        WriteToFile(orders);
        return order.Id;
    }

    public bool Delete(Guid id)
    {
        var orders = ReadFromFile();
        for (int i = 0; i < orders.Count; i++)
        {
            if (orders[i].Id == id)
            {
                orders.RemoveAt(i);
                WriteToFile(orders);
                return true;
            }
        }
        return false;
    }

    public List<Order> GetAll()
    {
        return ReadFromFile();
    }

    public Order? GetById(Guid id)
    {
        var orders = ReadFromFile();
        foreach (var order in orders)
        {
            if (order.Id == id)
            {
                return order;
            }
        }
        return null;
    }

    public bool Update(Order order)
    {
        var orders = ReadFromFile();
        for (int i = 0; i < orders.Count; i++)
        {
            if (orders[i].Id == order.Id)
            {
                orders[i] = order;
                WriteToFile(orders);
                return true;
            }
        }
        return false;
    }
}
