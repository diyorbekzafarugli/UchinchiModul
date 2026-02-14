using PostsSocialMedia.Api.Dtos.UserDto;
using PostsSocialMedia.Api.Entities;
using PostsSocialMedia.Api.Entities.User;

namespace PostsSocialMedia.Api.Services;

public interface IAuthService
{
    Result<Guid> Register(UserCreateDto userCreateDto);
    Result<string> LoginUser(string userName, string password);
}
