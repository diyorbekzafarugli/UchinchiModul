using StudentCoursePlatform.Application.DTOs.Users.Requests;
using StudentCoursePlatform.Application.DTOs.Users.Responses;
using StudentCoursePlatform.Domain.Entities;

namespace StudentCoursePlatform.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> RegisterAsync(UserRegisterDto userRegisterDto, CancellationToken cancellationToken);
    Task<Result<LoginResponseDto>> LoginAsync(UserLoginDto userLoginDto, CancellationToken cancellationToken);
    Task<Result<LoginResponseDto>> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
}
