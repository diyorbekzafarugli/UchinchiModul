using PostsSocialMedia.Api.Entities.Post;
using System.Security.Cryptography;

namespace PostsSocialMedia.Api.Repositories;

public interface IPostRepository
{
    void Add(Post post);
    Post? GetById(Guid id);
    IReadOnlyList<Post> GetAll();
    bool Update(Post postUpdated);
    bool Delete(Guid id);
    IReadOnlyList<Post> GetAllPosts(Guid userId);
}
