using PostsSocialMedia.Api.Entities.Follow;
using PostsSocialMedia.Api.Repositories;

public class FollowRepository : JsonRepository<Follow>, IFollowRepository
{
    public FollowRepository() : base("Followers") { }

    public async Task<List<Follow>> GetAllByUser(Guid userId)
    {
        var allFollows = await GetAll();
        return allFollows.Where(f => f.FollowingId == userId).ToList();
    }

    public async Task<List<Follow>> GetAllFollowings(Guid userId)
    {
        var allFollows = await GetAll();
        return allFollows.Where(f => f.FollowerId == userId).ToList();
    }
    public async Task<bool> IsFollowing(Guid followerId, Guid followingId)
    {
        var allFollows = await GetAll();
        return allFollows.Any(f => f.FollowerId == followerId && f.FollowingId == followingId);
    }
}