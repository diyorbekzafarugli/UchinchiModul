using MyProject.Interfaces;
using System.Text.Json;

namespace MyProject.Repositories;

public class JsonRepository<T> : IRepository<T> where T : IEntity
{
    private readonly string _filePath;
    public JsonRepository()
    {
        if (!Directory.Exists("Data")) Directory.CreateDirectory("Data");

        _filePath = Path.Combine("Data", typeof(T).Name.ToLower() + "s.json");
    }
    private List<T> ReadFromFile()
    {
        if (!File.Exists(_filePath)) return new List<T>();
        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }

    private void WriteToFile(List<T> entity)
    {
        var json = JsonSerializer.Serialize(entity, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_filePath, json);
    }

    public Guid Add(T entity)
    {
        var entities = ReadFromFile();

        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;

        entities.Add(entity);
        WriteToFile(entities);
        return entity.Id;
    }

    public bool Delete(Guid id)
    {
        var entities = ReadFromFile();

        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i].Id == id)
            {
                entities.RemoveAt(i);
                WriteToFile(entities);
                return true;
            }
        }
        return false;
    }

    public List<T> GetAll()
    {
        var entities = ReadFromFile();
        return entities;
    }

    public T? GetById(Guid id)
    {
        var entities = ReadFromFile();
        foreach (var entity in entities)
        {
            if (entity.Id == id)
            {
                return entity;
            }
        }
        return default;
    }

    public bool Update(T entity)
    {
        var entities = ReadFromFile();

        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i].Id == entity.Id)
            {
                entity.CreatedAt = entities[i].CreatedAt;

                entities[i] = entity;
                WriteToFile(entities);
                return true;
            }
        }
        return false;
    }



}
