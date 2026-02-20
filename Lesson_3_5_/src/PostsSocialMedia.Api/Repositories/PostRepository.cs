using PostsSocialMedia.Api.Entities.Post;

namespace PostsSocialMedia.Api.Repositories;

public class PostRepository : JsonRepository<Post>, IPostRepository
{
    public PostRepository() : base("Posts")
    {

    }

    public async Task<List<Post>> GetPostsByUserId(Guid userId)
    {
        var posts = await GetAll();
        return posts.Where(u => u.UserId == userId)
                    .ToList();
    }
}
