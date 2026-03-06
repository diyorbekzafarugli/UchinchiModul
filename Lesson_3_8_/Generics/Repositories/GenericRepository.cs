using Generics.Entities;
using System.Text.Json;

namespace Generics.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions options = new() { WriteIndented = true };

    public GenericRepository(string fileName)
    {
        var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        Directory.CreateDirectory(dir);
        _filePath = Path.Combine(dir, $"{fileName}.json");

        if (!File.Exists(_filePath)) File.WriteAllText(_filePath, "[]");
    }

    private void SaveToFile(List<T> items)
    {
        var json = JsonSerializer.Serialize<List<T>>(items, options);
        var dir = Path.GetDirectoryName(_filePath)!;

        var tempPath = Path.Combine(dir, $"{Path.GetFileNameWithoutExtension(_filePath)}_{Guid.NewGuid()}.tmp");

        File.WriteAllText(tempPath, json);
        File.Move(tempPath, _filePath, overwrite: true);
    }

    private List<T> ReadFromFile()
    {
        var json = File.ReadAllText(_filePath);
        if (string.IsNullOrWhiteSpace(json)) return [];

        return JsonSerializer.Deserialize<List<T>>(json, options) ?? [];

    }
    public void Add(T item)
    {
        var items = ReadFromFile();
        items.Add(item);
        SaveToFile(items);
    }

    public List<T> GetAll()
    {
        return ReadFromFile();
    }

    public bool Remove(Guid id)
    {
        var items = ReadFromFile();
        int index = items.FindIndex(x => x.Id == id);

        if (index < 0) return false;
        items.RemoveAt(index);
        return true;
    }
}
