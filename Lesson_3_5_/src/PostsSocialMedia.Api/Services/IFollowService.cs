using PostsSocialMedia.Api.Dtos.FollowDto;
using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Services;

public interface IFollowService
{
    Result<Guid> Add(Guid currentUserId, FollowAddDto followAddDto);
    Result<FollowGetDto> GetById(Guid currentUserId, Guid id);
    Result<List<FollowGetDto>> GetUsersByName(Guid currentUserId, string searchTerm, int page, int pageSize);
    Result<List<FollowGetDto>> GetAll(Guid currentUserId);
    Result<bool> Delete(Guid currentUserId, Guid id);
}
