using Microsoft.IdentityModel.Tokens;
using PostsSocialMedia.Api.Dtos.UserDto;
using PostsSocialMedia.Api.Entities;
using PostsSocialMedia.Api.Entities.User;
using PostsSocialMedia.Api.Repositories;
using System.IdentityModel.Tokens.Jwt;
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

    public async Task<Result<Guid>> RegisterAsync(UserCreateDto userCreateDto)
    {
        if (string.IsNullOrWhiteSpace(userCreateDto.UserName))
            return Result<Guid>.Fail("Foydalanuvchi nomi bo'sh bo'lishi mumkin emas");

        var existingUser = await _userRepository.GetByUserName(userCreateDto.UserName);
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

        if (userCreateDto.DateOfBirth.AddYears(14) > DateTime.UtcNow)
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

        await _userRepository.Add(user);
        return Result<Guid>.Ok(user.Id);
    }

    public async Task<Result<string>> LoginUserAsync(string userName, string password)
    {
        var user = await _userRepository.GetByUserName(Normalize(userName));
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
        { error = "Parolning boshi yoki oxirida bo'sh joy bo'lishi mumkin emas"; return false; }

        if (password.Length < 8 || password.Length > 64)
        { error = "Parolda kamida 8 tadan 64 tagacha belgi bo'lishi kerak"; return false; }

        if (password.Any(char.IsWhiteSpace))
        { error = "Parolda bo'sh joylar bo'lishi mumkin emas"; return false; }

        if (!password.Any(char.IsUpper))
        { error = "Parolda katta harf ishtirok etishi kerak"; return false; }

        if (!password.Any(char.IsLower))
        { error = "Parolda kichik harf ishtirok etishi kerak"; return false; }

        if (!password.Any(char.IsDigit))
        { error = "Parolda raqam ishtirok etishi kerak"; return false; }

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
        { error = "Parolda maxsus belgi ishtirok etishi kerak"; return false; }

        return true;
    }

    private string GenerateJwtToken(User user)
    {
        var secretKey = _configuration["JwtOptions:SecretKey"]!;
        var issuer = _configuration["JwtOptions:Issuer"];
        var audience = _configuration["JwtOptions:Audience"];

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}