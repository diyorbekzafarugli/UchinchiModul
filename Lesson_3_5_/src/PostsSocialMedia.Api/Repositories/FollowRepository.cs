using PostsSocialMedia.Api.Entities.Follow;

namespace PostsSocialMedia.Api.Repositories;

public class FollowRepository : JsonRepository<Follow>, IFollowRepository
{
    public FollowRepository() : base("Followers")
    {
        
    }

    public List<Follow> GetAllByUser(Guid userId)
    {
        var query = GetAll()
            .Where(f => f.FollowingId == userId)
            .ToList();

        return query;
    }

    public List<Follow> GetAllFollowings(Guid userId)
    {
        var query = GetAll()
            .Where(f => f.FollowerId == userId)
            .ToList();

        return query;
    }

    public bool IsFollowing(Guid followerId, Guid followingId)
    {
        var isFollowed = GetAll()
                    .Any(f => f.FollowerId == followerId && f.FollowingId == followingId);
        return isFollowed;
    }
}
