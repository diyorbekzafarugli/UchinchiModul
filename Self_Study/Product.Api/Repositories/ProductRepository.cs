using Product.Api.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Product.Api.Repositories;

public class ProductRepository : IRepositoriy<Product.Api.Entities.Product>
{
    private readonly string _filePath;
    public ProductRepository()
    {
        if (!Directory.Exists("Data")) Directory.CreateDirectory("Data");
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "products.json");
    }

    private List<Entities.Product> ReadFromFile()
    {
        if (!File.Exists(_filePath)) return new List<Entities.Product>();
        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Entities.Product>>(json) ?? new List<Entities.Product>();
    }

    private void WriteToFile(List<Entities.Product> products)
    {
        var json = JsonSerializer.Serialize(products, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(_filePath, json);
    }
    public Guid Add(Entities.Product product)
    {
        var products = ReadFromFile();

        products.Add(product);
        WriteToFile(products);
        return product.Id;
    }

    public bool Delete(Guid id)
    {
        var products = ReadFromFile();
        for (int i = 0; i < products.Count; i++)
        {
            if (products[i].Id == id)
            {
                products.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public List<Entities.Product> GetAll()
    {
        var products = ReadFromFile();
        return products;
    }

    public Entities.Product? GetById(Guid id)
    {
        if (id == Guid.Empty) return null;
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

    public bool Update(Entities.Product product)
    {
        var products = ReadFromFile();
        for (int i = 0; i < products.Count; i++)
        {
            if (products[i].Id == product.Id)
            {
                products[i] = product;
                WriteToFile(products);
                return true;
            }
        }
        return false;
    }
}
