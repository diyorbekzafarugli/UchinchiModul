using DemoTelegramBot.Dtos;
using DemoTelegramBot.Entities;

namespace DemoTelegramBot.Services;

public interface IUserService
{
    Result<bool> CheckAccess(Guid userId);

    Result<UserGetDto> GetById(Token token, Guid userId);
    Result<UserGetDto> GetByUserName(Token token, string userName);
    Result<IReadOnlyList<UserGetDto>> GetAll(Token token);

    Result<bool> UpdateProfile(Token token, string userName, string fullName, DateTime dateOfBirth);
    Result<bool> UpdateRole(Token token, Guid userId, UserRole role);
    Result<bool> ChangePassword(Token token, string newPassword);

    Result<bool> BlockUser(Token token, Guid userId, string? reason, int blockedUntilDays);
    Result<bool> UnblockUser(Token token, Guid userId);

    Result<bool> Delete(Token token, Guid userId);
}
