using PostsSocialMedia.Api.Dtos.FollowDto;
using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Services;

public interface IFollowService
{
    Task<Result<Guid>> Add(Guid currentUserId, FollowAddDto followAddDto);

    Task<Result<FollowGetDto>> GetById(Guid currentUserId, Guid id);

    Task<Result<List<FollowGetDto>>> GetUsersByName(Guid currentUserId, string searchTerm, int page, int pageSize);

    Task<Result<List<FollowGetDto>>> GetAll(Guid currentUserId);

    Task<Result<bool>> Delete(Guid currentUserId, Guid id);
}