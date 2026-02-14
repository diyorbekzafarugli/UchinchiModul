using PostsSocialMedia.Api.Dtos.UserDto;
using PostsSocialMedia.Api.Entities;
using PostsSocialMedia.Api.Entities.User;

namespace PostsSocialMedia.Api.Services;

public interface IUserService
{
    Result<UserGetDto> GetById(Guid id);
    Result<bool> Update(Guid currentUserId, UserUpdateDto userUpdateDto);
    Result<List<UserGetDto>> Search(string searchTerm, int page, int pageSize);
    Result<bool> Delete(Guid id);
    Result<UserGetDto> GetByUserName(string userName);
}
