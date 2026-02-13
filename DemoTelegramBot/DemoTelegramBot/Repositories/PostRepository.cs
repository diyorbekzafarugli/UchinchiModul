using DemoTelegramBot.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace DemoTelegramBot.Repositories;

public class PostRepository : IPostRepository
{
    private readonly string _filePath;

    private static readonly object _fileLock = new();
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public PostRepository()
    {
        var directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        Directory.CreateDirectory(directoryPath);

        _filePath = Path.Combine(directoryPath, "Posts.json");
        if (!File.Exists(_filePath))
            File.WriteAllText(_filePath, "[]");
    }

    private List<Post> ReadAllPosts_NoLock()
    {
        try
        {
            if (!File.Exists(_filePath)) return new List<Post>();
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Post>>(json) ?? new List<Post>();
        }
        catch
        {
            return new List<Post>();
        }
    }

    private void WriteAllPosts_NoLock(List<Post> posts)
    {
        var json = JsonSerializer.Serialize(posts, _jsonOptions);

        var dir = Path.GetDirectoryName(_filePath)!;
        var tempPath = Path.Combine(dir, $"{Path.GetFileNameWithoutExtension(_filePath)}_{Guid.NewGuid():N}.tmp");

        File.WriteAllText(tempPath, json);
        File.Move(tempPath, _filePath, overwrite: true);
    }

    public Guid Add(Post post)
    {
        lock (_fileLock)
        {
            var posts = ReadAllPosts_NoLock();
            posts.Add(post);
            WriteAllPosts_NoLock(posts);
            return post.PostId;
        }
    }

    public bool Delete(Guid userId, Guid postId)
    {
        lock (_fileLock)
        {
            var posts = ReadAllPosts_NoLock();
            var removed = posts.RemoveAll(p => p.UserId == userId && p.PostId == postId);
            if (removed == 0) return false;

            WriteAllPosts_NoLock(posts);
            return true;
        }
    }

    public IReadOnlyList<Post> GetAll()
    {
        lock (_fileLock)
        {
            var posts = ReadAllPosts_NoLock();
            return posts.AsReadOnly();
        }
    }

    public Post? GetById(Guid userId, Guid postId)
    {
        lock (_fileLock)
        {
            var posts = ReadAllPosts_NoLock();
            return posts.SingleOrDefault(p => p.UserId == userId && p.PostId == postId);
        }
    }

    public List<Post> GetByUserId(Guid userId)
    {
        lock (_fileLock)
        {
            var posts = ReadAllPosts_NoLock();
            return posts.Where(p => p.UserId == userId).ToList();
        }
    }

    public bool Update(Guid userId, Guid postId, string title, string content, DateTime updatedAt)
    {
        lock (_fileLock)
        {
            var posts = ReadAllPosts_NoLock();
            var userPost = posts.SingleOrDefault(p => p.UserId == userId && p.PostId == postId);
            if (userPost is null) return false;

            userPost.Title = title?.Trim() ?? string.Empty;
            userPost.Content = content?.Trim() ?? string.Empty;
            userPost.UpdatedAt = updatedAt;

            WriteAllPosts_NoLock(posts);
            return true;
        }
    }
}
