using SelfStudy.Entities;
using System.Text.Json;

namespace SelfStudy.Repositories;

public class ProductRepository : IRepository<Product>
{
    private readonly string _filePath;
    public ProductRepository()
    {
        if (!Directory.Exists("Data"))
            Directory.CreateDirectory("Data");

        _filePath = Path.Combine("Data", "products.json");
    }

    private List<Product> ReadFromFile()
    {
        if (!File.Exists(_filePath)) return new List<Product>();

        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Product>>(json) ?? new List<Product>();
    }

    private void WriteToFile(List<Product> products)
    {
        var json = JsonSerializer.Serialize(products, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_filePath, json);
    }
    public Guid Add(Product entity)
    {
        var products = ReadFromFile();

        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        products.Add(entity);

        WriteToFile(products);
        return entity.Id;
    }

    public bool Delete(Guid id)
    {
        var products = ReadFromFile();
        for (int i = 0; i < products.Count; i++)
        {
            if (products[i].Id == id)
            {
                products.RemoveAt(i);
                WriteToFile(products);
                return true;
            }
        }
        return false;
    }

    public List<Product> GetAll()
    {
        return ReadFromFile();
    }

    public Product? GetById(Guid id)
    {
        var products = ReadFromFile();

        foreach (var product in products)
        {
            if (product.Id == id)
            {
                return product;
            }
        }
        return null;
    }

    public bool Update(Product entity)
    {
        var products = ReadFromFile();
        var product = products.FirstOrDefault(p => p.Id == entity.Id);

        if (product == null) return false;

        product.Name = entity.Name;
        product.Price = entity.Price;
        product.Quantity = entity.Quantity;

        WriteToFile(products);
        return true;
    }
}
