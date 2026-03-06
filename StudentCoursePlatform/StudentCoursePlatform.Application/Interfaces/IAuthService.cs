using StudentCoursePlatform.Application.DTOs.Users;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> RegisterAsync(UserRegisterDto userRegisterDto);
    Task<Result<LoginResponseDto>> LoginAsync(UserLoginDto userLoginDto);

}
