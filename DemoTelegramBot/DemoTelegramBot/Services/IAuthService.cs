using DemoTelegramBot.Dtos;
using DemoTelegramBot.Entities;

namespace DemoTelegramBot.Services;

public interface IAuthService
{
    public Result<Token> Register(UserRegisterDto userRegisterDto);
    public Result<Token> Login(string userName, string password);
    public Result<bool> EnsureAccess(Token? token);
}
