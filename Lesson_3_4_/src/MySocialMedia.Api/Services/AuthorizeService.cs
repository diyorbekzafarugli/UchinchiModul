using MySocialMedia.Api.Dtos;
using MySocialMedia.Api.Entities;
using MySocialMedia.Api.Repositories;
using MySocialMedia.Api.Security;

namespace MySocialMedia.Api.Services;

public class AuthorizeService : IAuthorizeService
{
    private readonly IUserRepository _repository;

    public AuthorizeService(IUserRepository repository)
    {
        _repository = repository;
    }

    public Result<Token> Create(UserRegisterDto userRegisterDto)
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
            RegisteredAt = DateTime.UtcNow,
            Role = UserRole.User,
            IsBlocked = false
        };

        user.Password = PasswordHelper.Hash(user, userRegisterDto.Password);

        var res = _repository.Create(user);
        if (!res) return Result<Token>.Fail("UserName band. Boshqasini tanlang.");

        var token = new Token { UserId = user.UserId, Role = user.Role };
        return Result<Token>.Ok(token);
    }

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
