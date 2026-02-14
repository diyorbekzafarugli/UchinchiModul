using PostsSocialMedia.Api.Entities.Post;

namespace PostsSocialMedia.Api.Repositories;

public class PostRepository : JsonRepository<Post>, IPostRepository
{
    public PostRepository() : base("Posts")
    {

    }

    public IReadOnlyList<Post> GetAllPosts(Guid userId)
    {
        return GetAll()
            .Where(u => u.UserId == userId)
            .ToList()
            .AsReadOnly();
    }
}
