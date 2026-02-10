using MySocialMedia.Api.Entities;

namespace MySocialMedia.Api.Repositories;

public interface IUserRepository
{
    bool Create(User user);

    User? GetById(Guid userId);
    User? GetByUserName(string userName);
    IReadOnlyList<User> GetAll();

    bool UpdateProfile(Guid userId, string userName, string fullName, DateTime dateOfBirth);
    bool UpdateRole(Guid userId, UserRole role);
    bool ChangePassword(Guid userId, string newPassword);

    bool BlockUser(Guid userId, string? reason, DateTime blockedAt, DateTime blockedUntil);
    bool UnblockUser(Guid userId, DateTime updatedAt);

    bool Delete(Guid userId);
}
