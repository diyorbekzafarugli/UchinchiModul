using PostsSocialMedia.Api.Dtos.UserDto;
using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Services;

public interface IAuthService
{
    Task<Result<Guid>> RegisterAsync(UserCreateDto userCreateDto);
    Task<Result<string>> LoginUserAsync(string userName, string password);
}