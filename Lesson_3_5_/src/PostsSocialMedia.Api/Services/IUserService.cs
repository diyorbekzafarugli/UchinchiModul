using PostsSocialMedia.Api.Dtos.UserDto;
using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Services;

public interface IUserService
{
    Task<Result<UserGetDto>> GetByIdAsync(Guid id);

    Task<Result<bool>> UpdateAsync(Guid currentUserId, UserUpdateDto userUpdateDto);

    Task<Result<List<UserGetDto>>> SearchAsync(string searchTerm, int page, int pageSize);

    Task<Result<bool>> DeleteAsync(Guid id);

    Task<Result<UserGetDto>> GetByUserNameAsync(string userName);
}