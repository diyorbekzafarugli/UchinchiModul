using PostsSocialMedia.Api.Entities.Follow;

namespace PostsSocialMedia.Api.Repositories;

public interface IFollowRepository
{
    Task Add(Follow follow);
    Task<Follow?> GetById(Guid id);
    Task<IReadOnlyList<Follow>> GetAll();
    Task<List<Follow>> GetAllByUser(Guid userId);
    Task<List<Follow>> GetAllFollowings(Guid userId);
    Task<bool> IsFollowing(Guid followerId, Guid followingId);
    Task Update(Follow followUpdated);
    Task Delete(Guid id);
}
