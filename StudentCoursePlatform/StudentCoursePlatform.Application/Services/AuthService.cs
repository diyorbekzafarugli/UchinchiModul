using StudentCoursePlatform.Application.Abstractions.Repositories;
using StudentCoursePlatform.Application.DTOs.Users;
using StudentCoursePlatform.Application.Interfaces;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    public Task<Result<LoginResponseDto>> LoginAsync(UserLoginDto userLoginDto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<AuthResponseDto>> RegisterAsync(UserRegisterDto userRegisterDto)
    {
        throw new NotImplementedException();
    }
}
