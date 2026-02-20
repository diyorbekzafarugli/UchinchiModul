using PostsSocialMedia.Api.Entities;
using PostsSocialMedia.Api.Repositories;
using System.Text.Json;

public class JsonRepository<T> : IJsonRepository<T> where T : class, IEntity
{
    private readonly string _filePath;
    // Lock o'rniga SemaphoreSlim ishlatamiz, chunki lock ichida 'await' ishlatib bo'lmaydi
    protected static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public JsonRepository(string fileName)
    {
        var directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        Directory.CreateDirectory(directoryPath);
        _filePath = Path.Combine(directoryPath, $"{fileName}.json");

        if (!File.Exists(_filePath)) File.WriteAllText(_filePath, "[]");
    }

    // Asinxron o'qish
    protected async Task<List<T>> ReadAll_NoLock()
    {
        try
        {
            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
        catch { return new List<T>(); }
    }

    // Asinxron yozish
    protected async Task SaveAll_NoLock(List<T> items)
    {
        var json = JsonSerializer.Serialize(items, _jsonOptions);
        var dir = Path.GetDirectoryName(_filePath)!;

        // Vaqtinchalik fayl nomi (sizning mantiqingiz bo'yicha)
        var tempPath = Path.Combine(dir, $"{Path.GetFileNameWithoutExtension(_filePath)}_{Guid.NewGuid():N}.tmp");

        try
        {
            // 1. Ma'lumotni oldin vaqtinchalik faylga asinxron yozamiz
            await File.WriteAllTextAsync(tempPath, json);

            // 2. Agar hammasi muvaffaqiyatli bo'lsa, uni asosiy faylga almashtiramiz
            // Bu operatsiya operatsion tizim darajasida juda tez va xavfsiz bajariladi
            File.Move(tempPath, _filePath, overwrite: true);
        }
        catch (Exception)
        {
            // Agar xato bo'lsa, vaqtinchalik faylni o'chirib tashlaymiz
            if (File.Exists(tempPath)) File.Delete(tempPath);
            throw; // Xatoni yuqoriga uzatamiz
        }
    }

    public async Task Add(T item)
    {
        await _semaphore.WaitAsync(); // Faylni band qilamiz
        try
        {
            var items = await ReadAll_NoLock();
            items.Add(item);
            await SaveAll_NoLock(items);
        }
        finally { _semaphore.Release(); } // Faylni bo'shatamiz
    }

    public async Task<T?> GetById(Guid id)
    {
        await _semaphore.WaitAsync();
        try
        {
            var items = await ReadAll_NoLock();
            return items.SingleOrDefault(x => x.Id == id);
        }
        finally { _semaphore.Release(); }
    }

    public async Task<IReadOnlyList<T>> GetAll()
    {
        await _semaphore.WaitAsync();
        try
        {
            var items = await ReadAll_NoLock();
            return items.AsReadOnly();
        }
        finally { _semaphore.Release(); }
    }

    public async Task Update(T updatedItem)
    {
        await _semaphore.WaitAsync();
        try
        {
            var items = await ReadAll_NoLock();
            var index = items.FindIndex(x => x.Id == updatedItem.Id);
            if (index != -1)
            {
                items[index] = updatedItem;
                await SaveAll_NoLock(items);
            }
        }
        finally { _semaphore.Release(); }
    }

    public async Task Delete(Guid id)
    {
        await _semaphore.WaitAsync();
        try
        {
            var items = await ReadAll_NoLock();
            var item = items.SingleOrDefault(x => x.Id == id);
            if (item != null)
            {
                items.Remove(item);
                await SaveAll_NoLock(items);
            }
        }
        finally { _semaphore.Release(); }
    }
}