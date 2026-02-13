using Microsoft.AspNetCore.Mvc;
using PostsSocialMedia.Api.Entities;
using System.Text.Json;

namespace PostsSocialMedia.Api.Repositories;

public class JsonRepository<T> : IJsonRepository<T> where T : class, IEntity
{
    protected readonly string _filePath;

    protected static readonly object _fileLock = new();

    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public JsonRepository(string fileName)
    {
        var directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        Directory.CreateDirectory(directoryPath);

        _filePath = Path.Combine(directoryPath, $"{fileName}.json");

        if (!File.Exists(_filePath)) File.WriteAllText(_filePath, "[]");
    }

    private void SaveAll_NoLock(List<T> items)
    {
        var json = JsonSerializer.Serialize(items, _jsonOptions);
        var dir = Path.GetDirectoryName(_filePath)!;
        var tempPath = Path.Combine(dir, $"{Path.GetFileNameWithoutExtension(_filePath)}_{Guid.NewGuid():N}.tmp");

        File.WriteAllText(tempPath, json);
        File.Move(tempPath, _filePath, overwrite: true);
    }

    private List<T> ReadAll_NoLock()
    {
        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
        catch
        {
            return new List<T>();
        }
    }

    public void Add(T item)
    {
        lock (_fileLock)
        {
            var items = ReadAll_NoLock();
            items.Add(item);
            SaveAll_NoLock(items);
        }
    }

    public T? GetById(Guid id)
    {
        lock (_fileLock)
        {
            return ReadAll_NoLock().SingleOrDefault(x => x.Id == id);
        }
    }

    public IReadOnlyList<T> GetAll()
    {
        lock (_fileLock)
        {
            return ReadAll_NoLock().AsReadOnly();
        }
    }

    public bool Delete(Guid id)
    {
        lock (_fileLock)
        {
            var items = ReadAll_NoLock();
            var removedCount = items.RemoveAll(x => x.Id == id);
            if (removedCount > 0)
            {
                SaveAll_NoLock(items);
                return true;
            }
            return false;
        }
    }

    public bool Update(T updatedItem)
    {
        lock (_fileLock)
        {
            var items = ReadAll_NoLock();
            var index = items.FindIndex(x => x.Id == updatedItem.Id);

            if (index == -1) return false;

            items[index] = updatedItem;
            SaveAll_NoLock(items);
            return true;
        }
    }
}
