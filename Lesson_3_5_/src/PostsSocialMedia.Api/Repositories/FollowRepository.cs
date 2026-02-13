using PostsSocialMedia.Api.Entities;

namespace PostsSocialMedia.Api.Repositories;

public class FollowRepository : JsonRepository<Follow>, IFollowRepository
{
    public FollowRepository() : base("Followers")
    {
        
    }
}
