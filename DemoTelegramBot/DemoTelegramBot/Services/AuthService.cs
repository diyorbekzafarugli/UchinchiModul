using DemoTelegramBot.Bot.Core;
using DemoTelegramBot.Dtos;
using DemoTelegramBot.Entities;
using DemoTelegramBot.Repositories;
using DemoTelegramBot.Security;

namespace DemoTelegramBot.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _repository;

    public AuthService(IUserRepository repository)
    {
        _repository = repository;
    }

    public Result<Token> Register(UserRegisterDto userRegisterDto)
    {
        if (string.IsNullOrWhiteSpace(userRegisterDto.UserName) ||
             userRegisterDto.UserName.Length < 5 || userRegisterDto.UserName.Length > 64)
            return Result<Token>.Fail("UserName 5 tadan 64 tagacha belgidan iborat bo'lishi kerak");

        if (!IsValidPassword(userRegisterDto.Password, out var error))
            return Result<Token>.Fail(error);

        if (string.IsNullOrWhiteSpace(userRegisterDto.FullName) || userRegisterDto.FullName.Length < 5)
            return Result<Token>.Fail("FullName kamida 5 ta belgidan iborat bo'lishi kerak");

        if (userRegisterDto.DateOfBirth > DateTime.UtcNow)
            return Result<Token>.Fail("Tug'ilgan sana kelajak bo'lishi mumkin emas");

        if (userRegisterDto.DateOfBirth.AddYears(14) > DateTime.UtcNow)
            return Result<Token>.Fail("Foydalanuvchi 14 yoshdan katta bo'lishi kerak.");

        var user = new User
        {
            UserId = Guid.NewGuid(),
            UserName = userRegisterDto.UserName.Trim(),
            FullName = userRegisterDto.FullName.Trim(),
            DateOfBirth = userRegisterDto.DateOfBirth,
            Role = UserRole.User,
            RegisteredAt = DateTime.UtcNow,
            IsBlocked = false
        };

        user.Password = PasswordHelper.Hash(user, userRegisterDto.Password);

        var ok = _repository.Create(user);
        if (!ok) return Result<Token>.Fail("Bu username band.");

        return Result<Token>.Ok(new Token { UserId = user.UserId, Role = user.Role });
    }

    public Result<Token> Login(string userName, string password)
    {
        var user = _repository.GetByUserName(userName);
        if (user is null) return Result<Token>.Fail("User topilmadi.");

        if (user.IsBlocked && user.BlockedUntil is DateTime until && until > DateTime.UtcNow)
        {
            var reason = string.IsNullOrWhiteSpace(user.BlockReason) ? "" : $"<b>Sabab:</b> {Html.H(user.BlockReason)}\n";
            return Result<Token>.Fail($"Siz bloklangansiz.\n{reason}<b>Gacha:</b> <code>{until:u}</code>");
        }

        var ok = PasswordHelper.Verify(user, user.Password, password);
        if (!ok) return Result<Token>.Fail("Password xato.");

        return Result<Token>.Ok(new Token { UserId = user.UserId, Role = user.Role });
    }

    public Result<bool> EnsureAccess(Token? token)
    {
        if (token is null) return Result<bool>.Fail("Login bo‘ling.");

        var user = _repository.GetById(token.UserId);
        if (user is null) return Result<bool>.Fail("User topilmadi (o‘chirilgan bo‘lishi mumkin).");

        if (user.IsBlocked && user.BlockedUntil is DateTime until && until > DateTime.UtcNow)
            return Result<bool>.Fail($"Siz bloklangansiz. <b>Gacha:</b> <code>{until:u}</code>");

        return Result<bool>.Ok(true);
    }

    public static bool IsAdmin(Token? token)
        => token is not null && (token.Role is UserRole.Admin or UserRole.SuperAdmin);

    public static bool IsValidPassword(string password, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(password))
        { error = "Password bo'sh bo'lmasin."; return false; }

        if (password.Length < 8)
        { error = "Password kamida 8 ta belgidan iborat bo'lishi kerak."; return false; }

        if (password.Length > 64)
        { error = "Passworddagi belgilar soni 64 tadan oshmasligi kerak"; return false; }

        if (password.Any(char.IsWhiteSpace))
        { error = "Passwordda bo'sh joy bo'lishi mumkin emas."; return false; }

        bool hasUpperCase = password.Any(char.IsUpper);
        bool hasLowerCase = password.Any(char.IsLower);
        bool hasDigits = password.Any(char.IsDigit);
        bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        if (!hasLowerCase) { error = "Passwordda kamida bitta kichik harf bo'lishi kerak."; return false; }
        if (!hasUpperCase) { error = "Passwordda kamida bitta katta harf bo'lishi kerak."; return false; }
        if (!hasDigits) { error = "Passwordda raqamlar qatnashishi kerak."; return false; }
        if (!hasSpecial) { error = "Passwordda kamida bitta maxsus belgi bo'lishi kerak."; return false; }

        return true;
    }
}
