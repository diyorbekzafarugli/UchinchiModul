using PostsSocialMedia.Api.Entities.Follow;

namespace PostsSocialMedia.Api.Repositories;

public interface IFollowRepository
{
    void Add(Follow follow);
    Follow? GetById(Guid id);
    IReadOnlyList<Follow> GetAll();
    List<Follow> GetAllByUser(Guid userId);
    List<Follow> GetAllFollowings(Guid userId);
    bool IsFollowing(Guid followerId, Guid followingId);
    bool Update(Follow followUpdated);
    bool Delete(Guid id);
}
