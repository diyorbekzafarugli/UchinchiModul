using PostsSocialMedia.Api.Entities.Post;

namespace PostsSocialMedia.Api.Repositories;

public interface IPostRepository
{
    Task Add(Post post);
    Task<Post?> GetById(Guid id);
    Task<IReadOnlyList<Post>> GetAll();
    Task Update(Post postUpdated);
    Task Delete(Guid id);
    Task<List<Post>> GetPostsByUserId(Guid userId);
}
