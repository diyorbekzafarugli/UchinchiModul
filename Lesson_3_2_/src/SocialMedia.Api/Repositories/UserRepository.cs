using SocialMedia.Api.Entities;
using System.Text.Json;

namespace SocialMedia.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _users;
    private readonly string _posts;
    public UserRepository(IWebHostEnvironment env)
    {
        var _filePath = Path.Combine(env.ContentRootPath, "Data");
        Directory.CreateDirectory(_filePath);

        _users = Path.Combine(_filePath, "users.json");
        _posts = Path.Combine(_filePath, "posts.json");

        if (!File.Exists(_users)) File.WriteAllText(_users, "[]");
        if (!File.Exists(_posts)) File.WriteAllText(_posts, "[]");
    }
    private List<User> ReadUserFromFile()
    {
        if (!File.Exists(_users)) return new List<User>();
        var json = File.ReadAllText(_users);
        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }
    private void WriteUserToFile(List<User> users)
    {
        var json = JsonSerializer.Serialize(users, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(_users, json);
    }
    private List<Post> ReadPostFromFile()
    {
        if (!File.Exists(_posts)) return new List<Post>();
        var json = File.ReadAllText(_posts);
        return JsonSerializer.Deserialize<List<Post>>(json) ?? new List<Post>();
    }
    private void WritePostToFile(List<Post> posts)
    {
        var json = JsonSerializer.Serialize(posts, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(_posts, json);
    }
    public Guid AddPost(Guid userId, Post post)
    {
        var posts = ReadPostFromFile();
        posts.Add(post);
        WritePostToFile(posts);
        return post.PostId;
    }

    public Guid Create(User user)
    {
        var users = ReadUserFromFile();
        users.Add(user);
        WriteUserToFile(users);
        return user.UserId;
    }

    public bool Delete(Guid id)
    {
        if (id == Guid.Empty) return false;
        var users = ReadUserFromFile();
        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].UserId == id)
            {
                users.RemoveAt(i);
                WriteUserToFile(users);
                return true;
            }
        }
        return false;
    }

    public bool DeletePost(Guid userId, Guid postId)
    {
        if (userId == Guid.Empty || postId == Guid.Empty) return false;
        var posts = ReadPostFromFile();
        for (int i = 0; i < posts.Count; i++)
        {
            if (posts[i].UserId == userId && posts[i].PostId == postId)
            {
                posts.RemoveAt(i);
                WritePostToFile(posts);
                return true;
            }
        }
        return false;
    }

    public bool EditPost(Guid userId, Guid postId, string title, string content)
    {
        if (userId == Guid.Empty || postId == Guid.Empty) return false;

        var posts = ReadPostFromFile();
        for (int i = 0; i < posts.Count; i++)
        {
            if (posts[i].UserId == userId && posts[i].PostId == postId)
            {
                posts[i].Content = content;
                posts[i].Title = title;
                WritePostToFile(posts);
                return true;
            }
        }
        return false;
    }

    public User? GetUser(Guid id)
    {
        if (id == Guid.Empty) return null;
        var users = ReadUserFromFile();
        foreach (var user in users)
        {
            if (user.UserId == id)
            {
                return user;
            }
        }
        return null;
    }

    public List<User> GetUsers()
    {
        return ReadUserFromFile();
    }

    public bool Update(Guid id, User user)
    {
        if (id != user.UserId) return false;

        var users = ReadUserFromFile();
        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].UserId == id)
            {
                users[i] = user;
                WriteUserToFile(users);
                return true;
            }
        }
        return false;
    }

    public Post? GetPost(Guid userId, string userName)
    {
        if (userId == Guid.Empty) return null;
        var posts = ReadPostFromFile();
        var result = GetUser(userId);
        if (result is null) return null;
        foreach (var post in posts)
        {
            if(post.UserName == userName)
            {
                return post;
            }
        }
        return null;
    }

    public List<Post> GetAllPosts()
    {
        var posts = ReadPostFromFile();
        return posts;
    }
}
