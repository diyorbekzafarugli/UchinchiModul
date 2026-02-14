using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PostsSocialMedia.Api.Dtos.UserDto;
using PostsSocialMedia.Api.Entities;
using PostsSocialMedia.Api.Entities.User;
using PostsSocialMedia.Api.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace PostsSocialMedia.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }
    public Result<Guid> Register(UserCreateDto userCreateDto)
    {
        if (string.IsNullOrWhiteSpace(userCreateDto.UserName))
            return Result<Guid>.Fail("Foydalanuvhci nomi bo'sh bo'lishi mumkin emas");

        var existingUser = _userRepository.GetByUserName(userCreateDto.UserName);
        if (existingUser is not null)
            return Result<Guid>.Fail("Foydalanuvchi nomi band iltimos qaytadan kiriting");

        if (userCreateDto.UserName.Length > 64)
            return Result<Guid>.Fail("Foydalanuvchi nomi 64 ta belgidan oshmasligi kerak");

        if (!IsValidPassword(userCreateDto.Password, out var error))
            return Result<Guid>.Fail(error);

        if (string.IsNullOrWhiteSpace(userCreateDto.FirstName))
            return Result<Guid>.Fail("Foydalanuvchining ismi bo'sh bo'lmasligi kerak");

        if (userCreateDto.FirstName.Length > 64)
            return Result<Guid>.Fail("Foydalanuvchi ismi 64 ta belgidan oshmasligi kerak");

        if (string.IsNullOrWhiteSpace(userCreateDto.LastName))
            return Result<Guid>.Fail("Foydalanuvchining familiyasi bo'sh bo'lmasligi kerak");

        if (userCreateDto.LastName.Length > 64)
            return Result<Guid>.Fail("Foydalanuvchi familiyasi 64 ta belgidan oshmasligi kerak");

        if(userCreateDto.DateOfBirth.AddYears(14) > DateTime.UtcNow)
            return Result<Guid>.Fail("Foydalanuvchi 14 yoshdan oshgan bo'lishi kerak");

        var user = new User()
        {
            Id = Guid.NewGuid(),
            UserName = Normalize(userCreateDto.UserName),
            Password = BCrypt.Net.BCrypt.HashPassword(userCreateDto.Password),
            FirstName = Normalize(userCreateDto.FirstName),
            LastName = Normalize(userCreateDto.LastName),
            DateOfBirth = userCreateDto.DateOfBirth,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
            BlockedAt = null,
            BlockedUntil = null,
            BlockReason = null
        };

        _userRepository.Add(user);
        return Result<Guid>.Ok(user.Id);
    }
    public Result<string> LoginUser(string userName, string password)
    {
        var user = _userRepository.GetByUserName(Normalize(userName));
        if (user is null)
            return Result<string>.Fail("Login yoki parol xato");

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
        if (!isPasswordValid)
            return Result<string>.Fail("Login yoki parol xato");

        string token = GenerateJwtToken(user);
        return Result<string>.Ok(token);
    }

    public static string Normalize(string text) => (text ?? string.Empty).Trim();
    public static bool IsValidPassword(string password, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(password))
        { error = "Parol bo'sh bo'lishi mumkin emas"; return false; }

        if (password.Length != password.Trim().Length)
        { error = "Parolning boshi yoki oxirida bo'sh joy bo'lshi mumkin emas"; return false; }

        if (password.Length < 8 || password.Length > 64)
        { error = "Parolda kamida 8 tadan 64 tagacha belgi bo'lishi kerak"; return false; }

        if (password.Any(char.IsWhiteSpace))
        { error = "Parolda bo'sh joylar bo'lishi mumkin emas"; return false; }

        bool hasUpperCase = password.Any(char.IsUpper);
        bool hasLowerCase = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        if (!hasUpperCase)
        { error = "Parolda katta harf ishtirok etishi kerak"; return false; }

        if (!hasLowerCase)
        { error = "Parolda kichik harf ishtirok etishi kerak"; return false; }

        if (!hasDigit)
        { error = "Parolda raqam ishtirok etishi kerak"; return false; }

        if (!hasSpecial)
        { error = "Parolda maxsus belgi ishtirok etishi kerak"; return false; }

        return true;
    }

    private string GenerateJwtToken(User user)
    {
        // Appsettings.json dan ma'lumotlar 
        var secretKey = _configuration["JwtOptions:SecretKey"]!;
        var issuer = _configuration["JwtOptions:Issuer"];
        var audience = _configuration["JwtOptions:Audience"];

        // 1. Token ichida nima ma'lumotlar bo'lishi(Claims)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        // 2. Maxfiy kalitni kodirovkadan o'tkazamiz
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 3. Tokenning "Passport" qismini yig'amiz
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: creds
        );

        // 4. Tokenni matn ko'rinishiga o'tkazib qaytaramiz 
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
