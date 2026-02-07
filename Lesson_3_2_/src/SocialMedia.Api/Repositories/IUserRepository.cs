using SocialMedia.Api.Entities;

namespace SocialMedia.Api.Repositories;

public interface IUserRepository
{
    public Guid Create(User user);
    public List<User> GetUsers();
    public User? GetUser(Guid id);
    public bool Update(Guid id, User user);
    public bool Delete(Guid id);
    public Guid AddPost(Guid userId, Post post);
    public Post? GetPost(Guid userId, string userName);
    public List<Post> GetAllPosts();
    public bool EditPost(Guid userId, Guid postId, string title, string content);
    public bool DeletePost(Guid userId, Guid postId);
}
