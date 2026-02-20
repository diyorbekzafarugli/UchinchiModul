using MySocialMedia.Api.Entities;
using System.Text.Json;

namespace MySocialMedia.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _filePath;

    private static readonly object _fileLock = new();

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public UserRepository()
    {
        var directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        Directory.CreateDirectory(directoryPath);

        _filePath = Path.Combine(directoryPath, "Users.json");
        if (!File.Exists(_filePath)) File.WriteAllText(_filePath, "[]");
    }

    private static string Normalize(string value)
    {
        return (value ?? string.Empty).Trim();
    }

    private List<User> ReadFromFile_NoLock()
    {
        try
        {
            if (!File.Exists(_filePath)) return new List<User>();
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }
        catch
        {
            return new List<User>();
        }
    }

    private void SaveAllUsers_NoLock(List<User> users)
    {
        var json = JsonSerializer.Serialize(users, _jsonOptions);

        var dir = Path.GetDirectoryName(_filePath)!;
        var tempPath = Path.Combine(dir, $"{Path.GetFileNameWithoutExtension(_filePath)}_{Guid.NewGuid():N}.tmp");

        File.WriteAllText(tempPath, json);

        File.Move(tempPath, _filePath, overwrite: true);
    }

    public bool Create(User user)
    {
        lock (_fileLock)
        {
            var users = ReadFromFile_NoLock();

            var newUserName = Normalize(user.UserName);
            var exists = users.Any(u => Normalize(u.UserName).Equals(newUserName, StringComparison.OrdinalIgnoreCase));

            if (exists) return false;

            user.UserName = newUserName;
            users.Add(user);
            SaveAllUsers_NoLock(users);
            return true;
        }
    }

    public User? GetById(Guid userId)
    {
        lock (_fileLock)
        {
            var users = ReadFromFile_NoLock();
            return users.SingleOrDefault(u => u.UserId == userId);
        }
    }

    public User? GetByUserName(string userName)
    {
        lock (_fileLock)
        {
            var users = ReadFromFile_NoLock();
            var name = Normalize(userName);
            return users.FirstOrDefault(u => Normalize(u.UserName)
                .Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }

    public IReadOnlyList<User> GetAll()
    {
        lock (_fileLock)
        {
            var users = ReadFromFile_NoLock();
            return users.AsReadOnly();
        }
    }

    public bool UpdateProfile(Guid userId, string userName, string fullName, DateTime dateOfBirth)
    {
        lock (_fileLock)
        {
            var users = ReadFromFile_NoLock();
            var user = users.SingleOrDefault(u => u.UserId == userId);
            if (user is null) return false;

            var newUserName = Normalize(userName);

            var exists = users.Any(u => u.UserId != userId &&
                Normalize(u.UserName).Equals(newUserName, StringComparison.OrdinalIgnoreCase));
            if (exists) return false;

            user.UserName = newUserName;
            user.FullName = Normalize(fullName);
            user.DateOfBirth = dateOfBirth;
            user.UpdatedAt = DateTime.UtcNow;

            SaveAllUsers_NoLock(users);
            return true;
        }
    }

    public bool UpdateRole(Guid userId, UserRole role)
    {
        lock (_fileLock)
        {
            var users = ReadFromFile_NoLock();
            var user = users.SingleOrDefault(u => u.UserId == userId);
            if (user is null) return false;

            user.Role = role;
            user.UpdatedAt = DateTime.UtcNow;

            SaveAllUsers_NoLock(users);
            return true;
        }
    }

    public bool ChangePassword(Guid userId, string hashedPassword)
    {
        lock (_fileLock)
        {
            var users = ReadFromFile_NoLock();
            var user = users.SingleOrDefault(u => u.UserId == userId);
            if (user is null) return false;

            user.Password = hashedPassword;
            user.UpdatedAt = DateTime.UtcNow;

            SaveAllUsers_NoLock(users);
            return true;
        }
    }

    public bool BlockUser(Guid userId, string? reason, DateTime blockedAt, DateTime blockedUntil)
    {
        lock (_fileLock)
        {
            var users = ReadFromFile_NoLock();
            var user = users.SingleOrDefault(u => u.UserId == userId);
            if (user is null) return false;

            user.IsBlocked = true;
            user.BlockedAt = blockedAt;
            user.BlockReason = reason;
            user.BlockedUntil = blockedUntil;
            user.UpdatedAt = DateTime.UtcNow;

            SaveAllUsers_NoLock(users);
            return true;
        }
    }

    public bool UnblockUser(Guid userId, DateTime updatedAt)
    {
        lock (_fileLock)
        {
            var users = ReadFromFile_NoLock();
            var user = users.SingleOrDefault(u => u.UserId == userId);
            if (user is null) return false;

            user.IsBlocked = false;
            user.BlockedAt = null;
            user.BlockedUntil = null;
            user.BlockReason = null;
            user.UpdatedAt = updatedAt;

            SaveAllUsers_NoLock(users);
            return true;
        }
    }

    public bool Delete(Guid userId)
    {
        lock (_fileLock)
        {
            var users = ReadFromFile_NoLock();
            var removed = users.RemoveAll(u => u.UserId == userId);
            if (removed == 0) return false;

            SaveAllUsers_NoLock(users);
            return true;
        }
    }
}
