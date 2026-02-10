using Microsoft.AspNetCore.Http.HttpResults;
using MySocialMedia.Api.Dtos;
using MySocialMedia.Api.Entities;
using MySocialMedia.Api.Repositories;
using MySocialMedia.Api.Security;
using System.Collections.Generic;

namespace MySocialMedia.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Result<bool> CheckAccess(Guid userId)
    {
        if (userId == Guid.Empty)
            return Result<bool>.Fail("UserId xato kiritildi.");

        var user = _userRepository.GetById(userId);
        if (user is null)
            return Result<bool>.Fail("Ro'yxatdan o'tmagansiz.");

        if (!user.IsBlocked)
            return Result<bool>.Ok(true);


        if (user.BlockedUntil is not null && user.BlockedUntil <= DateTime.UtcNow)
        {
            _userRepository.UnblockUser(userId, DateTime.UtcNow);
            return Result<bool>.Ok(true);
        }

        var reason = string.IsNullOrWhiteSpace(user.BlockReason) ? "" : $" Sabab: {user.BlockReason}.";
        var until = user.BlockedUntil is null ? "" : $" Tugashi: {user.BlockedUntil:yyyy-MM-dd HH:mm} (UTC).";
        return Result<bool>.Fail($"Blocklangansiz.{reason}{until}");
    }

    private static UserGetDto ToDto(User u) => new()
    {
        UserId = u.UserId,
        UserName = u.UserName,
        FullName = u.FullName,
        DateOfBirth = u.DateOfBirth,
        RegisteredAt = u.RegisteredAt,
        UpdatedAt = u.UpdatedAt,
        IsBlocked = u.IsBlocked,
        BlockReason = u.BlockReason,
        BlockedAt = u.BlockedAt,
        BlockedUntil = u.BlockedUntil,
        Role = u.Role
    };

    public Result<UserGetDto> GetById(Token token, Guid userId)
    {
        var access = CheckAccess(token.UserId);
        if (!access.Success) return Result<UserGetDto>.Fail(access.Error!);

        if (userId == Guid.Empty) return Result<UserGetDto>.Fail("UserId xato kiritildi.");

        var user = _userRepository.GetById(userId);
        if (user is null) return Result<UserGetDto>.Fail("Foydalanuvchi topilmadi.");

        return Result<UserGetDto>.Ok(ToDto(user));
    }

    public Result<UserGetDto> GetByUserName(Token token, string userName)
    {
        var access = CheckAccess(token.UserId);
        if (!access.Success) return Result<UserGetDto>.Fail(access.Error!);

        if (string.IsNullOrWhiteSpace(userName))
            return Result<UserGetDto>.Fail("UserName bo'sh bo'lishi mumkin emas.");

        if (userName.Length < 5 || userName.Length > 64)
            return Result<UserGetDto>.Fail("UserName uzunligi 5..64 bo'lishi kerak.");

        var user = _userRepository.GetByUserName(userName);
        if (user is null) return Result<UserGetDto>.Fail("Foydalanuvchi topilmadi.");

        return Result<UserGetDto>.Ok(ToDto(user));
    }

    public Result<IReadOnlyList<UserGetDto>> GetAll(Token token)
    {
        var access = CheckAccess(token.UserId);
        if (!access.Success) return Result<IReadOnlyList<UserGetDto>>.Fail(access.Error!);

        var users = _userRepository.GetAll();
        if (users.Count == 0) return Result<IReadOnlyList<UserGetDto>>.Fail("Foydalanuvchilar topilmadi.");

        return Result<IReadOnlyList<UserGetDto>>.Ok(users.Select(ToDto).ToList());
    }

    public Result<bool> UpdateProfile(Token token, string userName, string fullName, DateTime dateOfBirth)
    {
        var access = CheckAccess(token.UserId);
        if (!access.Success) return Result<bool>.Fail(access.Error!);

        if (string.IsNullOrWhiteSpace(userName) || userName.Length < 5 || userName.Length > 64)
            return Result<bool>.Fail("UserName 5..64 bo'lishi kerak.");

        if (string.IsNullOrWhiteSpace(fullName) || fullName.Trim().Length < 5)
            return Result<bool>.Fail("FullName kamida 5 ta belgidan iborat bo'lishi kerak.");

        if (dateOfBirth > DateTime.UtcNow)
            return Result<bool>.Fail("Tug'ilgan sana kelajak bo'lishi mumkin emas.");

        if (dateOfBirth.AddYears(14) > DateTime.UtcNow)
            return Result<bool>.Fail("14 yoshdan katta bo'lishi kerak.");

        var ok = _userRepository.UpdateProfile(token.UserId, userName, fullName, dateOfBirth);
        return ok ? Result<bool>.Ok(true) : Result<bool>.Fail("Update bo'lmadi (username band yoki user topilmadi).");
    }

    public Result<bool> ChangePassword(Token token, string newPassword)
    {
        var access = CheckAccess(token.UserId);
        if (!access.Success) return Result<bool>.Fail(access.Error!);

        if (!AuthorizeService.IsValidPassword(newPassword, out var error))
            return Result<bool>.Fail(error);

        // Eslatma: agar sen hashing qilayotgan bo'lsang bu yerda hashed yubor
        var ok = _userRepository.ChangePassword(token.UserId, newPassword);
        return ok ? Result<bool>.Ok(true) : Result<bool>.Fail("Foydalanuvchi topilmadi.");
    }

    public Result<bool> UpdateRole(Token token, Guid userId, UserRole role)
    {
        var access = CheckAccess(token.UserId);
        if (!access.Success) return Result<bool>.Fail(access.Error!);

        // ✅ faqat SuperAdmin
        if (token.Role != UserRole.SuperAdmin)
            return Result<bool>.Fail("Sizda role o'zgartirish huquqi yo'q.");

        if (userId == Guid.Empty) return Result<bool>.Fail("UserId xato kiritildi.");
        if (role == UserRole.SuperAdmin) return Result<bool>.Fail("SuperAdmin berish taqiqlangan.");

        var target = _userRepository.GetById(userId);
        if (target is null) return Result<bool>.Fail("Foydalanuvchi topilmadi.");
        if (target.Role == UserRole.SuperAdmin) return Result<bool>.Fail("SuperAdmin'ni o'zgartirib bo'lmaydi.");

        var ok = _userRepository.UpdateRole(userId, role);
        return ok ? Result<bool>.Ok(true) : Result<bool>.Fail("Role yangilanmadi.");
    }

    public Result<bool> BlockUser(Token token, Guid userId, string? reason, int blockedUntilDays)
    {
        var access = CheckAccess(token.UserId);
        if (!access.Success) return Result<bool>.Fail(access.Error!);

        if (token.Role == UserRole.User)
            return Result<bool>.Fail("Sizda block qilish huquqi yo'q.");

        if (userId == Guid.Empty) return Result<bool>.Fail("UserId xato kiritildi.");
        if (blockedUntilDays < 1) return Result<bool>.Fail("BlockedUntil 1 kundan kam bo'lmasin.");
        if (reason is not null && reason.Length > 128) return Result<bool>.Fail("Reason 128 belgidan oshmasin.");

        var target = _userRepository.GetById(userId);
        if (target is null) return Result<bool>.Fail("Foydalanuvchi topilmadi.");
        if (target.Role == UserRole.SuperAdmin) return Result<bool>.Fail("SuperAdmin'ni block qilib bo'lmaydi.");

        var ok = _userRepository.BlockUser(userId, reason, DateTime.UtcNow, DateTime.UtcNow.AddDays(blockedUntilDays));
        return ok ? Result<bool>.Ok(true) : Result<bool>.Fail("Block bo'lmadi.");
    }

    public Result<bool> UnblockUser(Token token, Guid userId)
    {
        var access = CheckAccess(token.UserId);
        if (!access.Success) return Result<bool>.Fail(access.Error!);

        if (token.Role == UserRole.User)
            return Result<bool>.Fail("Sizda unblock qilish huquqi yo'q.");

        if (userId == Guid.Empty) return Result<bool>.Fail("UserId xato kiritildi.");

        var ok = _userRepository.UnblockUser(userId, DateTime.UtcNow);
        return ok ? Result<bool>.Ok(true) : Result<bool>.Fail("Unblock bo'lmadi (user topilmadi).");
    }

    public Result<bool> Delete(Token token, Guid userId)
    {
        var access = CheckAccess(token.UserId);
        if (!access.Success) return Result<bool>.Fail(access.Error!);

        if (userId == Guid.Empty) return Result<bool>.Fail("UserId xato kiritildi.");

        if (token.UserId != userId && token.Role != UserRole.SuperAdmin)
            return Result<bool>.Fail("Sizda o'chirish huquqi yo'q.");

        var ok = _userRepository.Delete(userId);
        return ok ? Result<bool>.Ok(true) : Result<bool>.Fail("User topilmadi.");
    }
}
